using System;
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
using static Common.Ri2Constants;

namespace Services
{
    public class UpdateService : IUpdateService
    {
        private const string DlFideZip = "Скачивается fide.zip...";
        private const string DlFideZipComplete = "fide.zip скачан.";
        private const string DlRcfXsl = "Скачивается rcf.xlsx...";
        private const string DlRcfXslComplete = "rcf.xlsx скачан.";
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
                progress?.Report(DlFideZip);
                await client.DownloadFileTaskAsync(new Uri(Settings.Current.FideUrl), FideZipPath)
                    .ConfigureAwait(false);
                progress?.Report(DlFideZipComplete);
                progress?.Report(DlRcfXsl);
                await client.DownloadFileTaskAsync(new Uri(Settings.Current.RcfUrl), RcfFilePath)
                    .ConfigureAwait(false);
                progress?.Report(DlRcfXslComplete);
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
            await Task.Run(() => CheckProfiles()).ConfigureAwait(false);
            using (var db = new Ri2Context())
            {
                var t = db.Info.SingleOrDefault(x => x.Name == InfoKeys.LastUpdate);
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

        private static void CheckProfiles()
        {
            using (var ri2 = new Ri2Context())
            {
                foreach (var profile in ri2.Profiles)
                {
                    if (profile.FideProfileId.HasValue && profile.RcfProfileId.HasValue) continue;
                    if (profile.RcfProfileId.HasValue)
                    {
                        if (profile.RcfProfile.FideProfileId.HasValue)
                            profile.FideProfile = profile.RcfProfile.FideProfile;
                        continue;
                    }

                    if (!profile.FideProfileId.HasValue) continue;
                    var id = profile.FideProfileId;
                    var rcf = ri2.RcfProfiles.SingleOrDefault(x => x.FideProfileId == id);
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
                    using (var ri2 = new Ri2Context())
                    {
                        if (fideId.HasValue)
                        {
                            var tf = ri2.FideProfiles.AsQueryable().FirstOrDefault(x => x.FideId == fideId.Value);
                            if (tf != null)
                            {
                                pr.FideProfileId = tf.Id;
                                pr.FideProfile = tf;
                            }
                        }

                        if (ri2.RcfProfiles.AsQueryable().Any(cp => cp.RcfId == pr.RcfId))
                        {
                            var t = ri2.RcfProfiles.Single(cp => cp.RcfId == pr.RcfId);
                            pr.Id = t.Id;
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
                    EFBatchOperation.For(db, db.RcfProfiles).InsertAll(add);
                }

                using (var db = new Ri2Context())
                {
                    EFBatchOperation.For(db, db.RcfProfiles).UpdateAll(mod,
                        x => x.ColumnsToUpdate(c => c.Name).ColumnsToUpdate(c => c.Birth).ColumnsToUpdate(c => c.Std)
                            .ColumnsToUpdate(c => c.Rpd).ColumnsToUpdate(c => c.Blz)
                            .ColumnsToUpdate(c => c.FideProfileId));
                }
            }
        }

        private static void ProcessFide()
        {
            var query = SimpleXmlStream.SimpleStreamAxis(FideFilePath, FideXmlElements.Player);
            var add = new List<FideProfile>(100000);
            var mod = new List<FideProfile>(100000);
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
                using (var ri2 = new Ri2Context())
                {
                    if (ri2.FideProfiles.AsQueryable().Any(cp => cp.FideId == pr.FideId))
                    {
                        var t = ri2.FideProfiles.Single(cp => cp.FideId == pr.FideId);
                        pr.Id = t.Id;
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
            }

            using (var db = new Ri2Context())
            {
                EFBatchOperation.For(db, db.FideProfiles).UpdateAll(mod,
                    x => x.ColumnsToUpdate(c => c.Name).ColumnsToUpdate(c => c.Birth).ColumnsToUpdate(c => c.Std)
                        .ColumnsToUpdate(c => c.Rpd).ColumnsToUpdate(c => c.Blz));
            }
        }
    }
}