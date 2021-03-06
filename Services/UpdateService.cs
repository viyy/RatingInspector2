﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common;
using DAL;
using EntityFramework.Utilities;
using Interfaces;
using Models;
using OfficeOpenXml;
using Services.Helpers;
using static Common.Ri2Constants;
using Exception = System.Exception;

namespace Services
{
    public class UpdateService : IUpdateService
    {
        private const string FideZip = "Скачивается fide.zip...";
        private const string FideZipComplete = "fide.zip скачан.";
        private const string RcfXsl = "Скачивается rcf.xlsx...";
        private const string RcfXslComplete = "rcf.xlsx скачан.";
        private const string FideUnzip = "Распаковка...";
        private const string FideProcess = "Обработка файла Fide...";
        private const string CompleteMsg = "Готово";
        private const string RcfProcess = "Обработка файла РШФ...";
        private const string Chk = "Синхронизация профилей...";

        public async Task UpdateAsync(IProgress<string> progress = null)
        {
            if (File.Exists(FideZipPath)) File.Delete(FideZipPath);

            if (File.Exists(RcfFilePath)) File.Delete(RcfFilePath);

            if (File.Exists(FideFilePath)) File.Delete(FideFilePath);
            using (var client = new WebClient())
            {
                progress?.Report(FideZip);
                await client.DownloadFileTaskAsync(new Uri(Settings.Current.FideUrl), FideZipPath)
                    .ConfigureAwait(false);
                progress?.Report(FideZipComplete);
                progress?.Report(RcfXsl);
                await client.DownloadFileTaskAsync(new Uri(Settings.Current.RcfUrl), RcfFilePath)
                    .ConfigureAwait(false);
                progress?.Report(RcfXslComplete);
            }

            progress?.Report(FideUnzip);
            ZipFile.ExtractToDirectory(FideZipPath, TmpPath);
            progress?.Report(FideProcess);
            /*
            <fideid>419214</fideid>
            <name></name>
            <country>ENG</country>
            <sex>M</sex>
            <title></title>
            <w_title></w_title>
            <o_title></o_title>
            <foa_title></foa_title>
            <rating>1736</rating>
            <games>0</games>
            <k>20</k>
            <rapid_rating>1587</rapid_rating>
            <rapid_games>10</rapid_games>
            <rapid_k>20</rapid_k>
            <blitz_rating></blitz_rating>
            <blitz_games></blitz_games>
            <blitz_k></blitz_k>
            <birthday></birthday>
            <flag></flag>
             */

            await Task.Run(() => ProcessFide()).ConfigureAwait(false);
            progress?.Report(CompleteMsg);
            progress?.Report(RcfProcess);
            await Task.Run(() => ProcessRcf()).ConfigureAwait(false);
            progress?.Report(Chk);
            CheckProfiles();
            using (var db = new Ri2Context())
            {
                var t = db.Info.FirstOrDefault(x => x.Name == InfoKeys.LastUpdate);
                if (t == null)
                {
                    t = new InfoValue
                    {
                        Name = InfoKeys.LastUpdate,
                        Value = DateTime.Now.ToShortDateString()
                    };
                    db.Info.Add(t);
                }
                else
                {
                    t.Value = DateTime.Now.ToShortDateString();
                }

                await db.SaveChangesAsync().ConfigureAwait(false);
            }

            progress?.Report(CompleteMsg);
        }

        public void CleanUp()
        {
            if (!Settings.Current.RemoveTmpFiles) return;
            try
            {
                File.Delete(FideZipPath);
                File.Delete(FideFilePath);
                File.Delete(RcfFilePath);
            }
            catch (Exception e)
            {
                File.AppendAllText("err.log",
                    DateTime.Now.ToShortTimeString() + "| UpdateService | CleanUp |" +e.Message +
                    Environment.NewLine);
            }
        }

        private static void CheckProfiles()
        {
            using (var ri2 = new Ri2Context())
            {
                foreach (var profile in ri2.Profiles.Include("RcfProfile").Include("FideProfile"))
                {
                    if (profile.FideProfileId.HasValue && profile.RcfProfileId.HasValue) continue;

                    if (profile.RcfProfileId.HasValue)
                    {
                        if (profile.RcfProfile.FideProfileId.HasValue)
                            profile.FideProfile = profile.RcfProfile.FideProfile;
                        continue;
                    }

                    if (!profile.FideProfileId.HasValue) continue;
                    var id = profile.FideProfileId.Value;
                    var rcf = ri2.RcfProfiles.FirstOrDefault(x => x.FideProfileId.Value == id);
                    if (rcf == null) continue;
                    profile.RcfProfile = rcf;
                }

                ri2.SaveChanges();
            }
        }


        private static void ProcessRcf()
        {
            var fi = new FileInfo(RcfFilePath);
            using (var pkg = new ExcelPackage(fi))
            {
                var ws = pkg.Workbook.Worksheets[1];
                var start = ws.Dimension.Start;
                var end = ws.Dimension.End;
                var add = new List<RcfProfile>(75000);
                var mod = new List<RcfProfile>(75000);
                Dictionary<int, int> tf;
                Dictionary<int, int> t;
                using (var db = new Ri2Context {Configuration = {AutoDetectChangesEnabled = false}})
                {
                    tf = db.FideProfiles.Select(x => new {x.FideId, x.Id})
                        .ToDictionary(o => o.FideId, o => o.Id);
                    t = db.RcfProfiles.Select(x => new {x.RcfId, x.Id})
                        .ToDictionary(o => o.RcfId, o => o.Id);
                }

                for (var i = start.Row + 1; i <= end.Row; i++)
                {
                    var pr = new RcfProfile
                    {
                        RcfId = ws.Cells[i, RcfColumns.RcfId].GetValue<int>(),
                        Name = ws.Cells[i, RcfColumns.Name].GetValue<string>(),
                        Birth = ws.Cells[i, RcfColumns.Birth].GetValue<string>() != ""
                            ? ws.Cells[i, RcfColumns.Birth].GetValue<int>()
                            : 0,
                        Std = ws.Cells[i, RcfColumns.Std].GetValue<string>() != ""
                            ? ws.Cells[i, RcfColumns.Std].GetValue<int>()
                            : 0,
                        Rpd = ws.Cells[i, RcfColumns.Rpd].GetValue<string>() != ""
                            ? ws.Cells[i, RcfColumns.Rpd].GetValue<int>()
                            : 0,
                        Blz = ws.Cells[i, RcfColumns.Blz].GetValue<string>() != ""
                            ? ws.Cells[i, RcfColumns.Blz].GetValue<int>()
                            : 0
                    };
                    if (pr.Birth < Settings.Current.BirthCutoff) continue;
                    var fideId = ws.Cells[i, RcfColumns.FideId].GetValue<int?>();

                    //using (var ri2 = new Ri2Context {Configuration = {AutoDetectChangesEnabled = false}})
                    {
                        if (fideId.HasValue)
                            if (tf.ContainsKey(fideId.Value))
                                pr.FideProfileId = tf[fideId.Value];

                        //var t = ri2.RcfProfiles.FirstOrDefault(cp => cp.RcfId == pr.RcfId);
                        if (t.ContainsKey(pr.RcfId))
                        {
                            pr.Id = t[pr.RcfId];
                            mod.Add(pr);
                        }
                        else
                        {
                            add.Add(pr);
                        }
                    }
                }

                using (var ri2 = new Ri2Context())
                {
                    EFBatchOperation.For(ri2, ri2.RcfProfiles).InsertAll(add);
                    EFBatchOperation.For(ri2, ri2.RcfProfiles).UpdateAll(mod,
                        x => x.ColumnsToUpdate(c => c.Name, c => c.Birth, c => c.Std, c => c.Rpd, c => c.Blz,
                            c => c.FideProfileId));
                    ri2.SaveChanges();
                }
            }
        }

        private static void ProcessFide()
        {
            var query = SimpleXmlStream.SimpleStreamAxis(FideFilePath, FideXmlElements.Player);
            var add = new List<FideProfile>(100000);
            var mod = new List<FideProfile>(100000);
            Dictionary<int, int> t;
            using (var db = new Ri2Context {Configuration = {AutoDetectChangesEnabled = false}})
            {
                t = db.FideProfiles.Select(x => new {x.FideId, x.Id})
                    .ToDictionary(o => o.FideId, o => o.Id);
            }

            foreach (var profile in query)
            {
                if (!Settings.Current.Filter.Contains(profile.Element(FideXmlElements.Country)?.Value)) continue;
                if (profile.Element(FideXmlElements.Name)?.Value == "") continue;
                var pr = new FideProfile
                {
                    Name = (profile.Element(FideXmlElements.Name)?.Value == ""
                               ? "_"
                               : profile.Element(FideXmlElements.Name)?.Value) ?? "_",
                    FideId = Convert.ToInt32((profile.Element(FideXmlElements.FideId)?.Value == ""
                                                 ? "0"
                                                 : profile.Element(FideXmlElements.FideId)?.Value) ?? "0"),
                    Std = Convert.ToInt32((profile.Element(FideXmlElements.Std)?.Value == ""
                                              ? "0"
                                              : profile.Element(FideXmlElements.Std)?.Value) ?? "0"),
                    Rpd = Convert.ToInt32((profile.Element(FideXmlElements.Rpd)?.Value == ""
                                              ? "0"
                                              : profile.Element(FideXmlElements.Rpd)?.Value) ?? "0"),
                    Blz = Convert.ToInt32((profile.Element(FideXmlElements.Blz)?.Value == ""
                                              ? "0"
                                              : profile.Element(FideXmlElements.Blz)?.Value) ?? "0"),
                    Birth = Convert.ToInt32((profile.Element(FideXmlElements.Birth)?.Value == ""
                                                ? "0"
                                                : profile.Element(FideXmlElements.Birth)?.Value) ?? "0")
                };
                if (pr.Birth < Settings.Current.BirthCutoff) continue;
                //File.AppendAllText("log.txt",pr.FideId+Environment.NewLine);
                //using (var ri2 = new Ri2Context {Configuration = {AutoDetectChangesEnabled = false}})
                {
                    //var t = ri2.FideProfiles.FirstOrDefault(cp => cp.FideId == pr.FideId);
                    //if (t != null)
                    if (t.ContainsKey(pr.FideId))
                    {
                        //pr.Id = t.Id;
                        pr.Id = t[pr.FideId];
                        mod.Add(pr);
                    }
                    else
                    {
                        add.Add(pr);
                    }
                }
            }

            using (var db = new Ri2Context())
            {
                EFBatchOperation.For(db, db.FideProfiles).InsertAll(add);
                db.SaveChanges();
            }

            using (var db = new Ri2Context())
            {
                EFBatchOperation.For(db, db.FideProfiles).UpdateAll(mod,
                    x => x.ColumnsToUpdate(c => c.Name, c => c.Birth, c => c.Std, c => c.Rpd, c => c.Blz));
                db.SaveChanges();
            }
        }
    }
}