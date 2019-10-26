using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using DAL;
using Interfaces;
using Models;
using PluginShared;
using Services.Helpers;
using Group = Models.Group;

namespace Services
{
    public class ImportService : BaseGroupListProvider, IImportService
    {
        private readonly List<IFileImporter> _plugins = new List<IFileImporter>();

        public ImportService()
        {
            _plugins.AddRange(PluginManager.GetPlugins<IFileImporter>());
            Logger.Log("ImportService", $"Registered {_plugins.Count} plugins");
        }

        public string GetFilters()
        {
            return string.Join("|", _plugins.Select(x => x.Filter));
        }

        public IEnumerable<int> LoadFromFile(string path)
        {
            try
            {
                Logger.Log("ImportService", $"Loading from {path}");
                var plugin = _plugins.FirstOrDefault(x => x.SupportedFormats.Any(path.EndsWith));
                if (plugin == null)
                {
                    Logger.Log("ImportService", $"Plugin for {path} not found", LogLevel.Error);
                }
                var res = plugin == null ? new List<int>() : plugin.Read(path).ToList();
                Logger.Log("ImportService", $"Loaded {res.Count} entries");
                return res;
            }
            catch (Exception ex)
            {
                Logger.Log("ImportService", $"Error: {ex.Message}", LogLevel.Error);
            }
            return new List<int>();
        }

        public IEnumerable<int> LoadFromUrl(string path)
        {
            try
            {
                Logger.Log("ImportService", $"Loading from {path}");
                var res = new List<int>();
                var htmlCode = "";
                using (var client = new WebClient())
                {
                    // htmlCode = await client.DownloadStringTaskAsync(new Uri(path)).ConfigureAwait(false);
                    htmlCode = client.DownloadString(path);
                }

                //Get Full Table
                const string pattern1 = @"<table.+<\/table>";
                //Get Ids
                const string pattern2 = @"<a href=""\/people\/(\d+)""";
                foreach (Match m in Regex.Matches(htmlCode, pattern1, RegexOptions.Multiline))
                {
                    foreach (Match p in Regex.Matches(m.Value, pattern2, RegexOptions.Multiline))
                    {
                        res.Add(Convert.ToInt32(p.Groups[1].Value));
                    }
                }

                Logger.Log("ImportService", $"Loaded {res.Count} entries");
                return res;
            }
            catch (Exception ex)
            {
                Logger.Log("ImportService", $"Error: {ex.Message}", LogLevel.Error);
            }
            return new List<int>();
        }

        public void ImportAsync(IEnumerable<int> ids, Group group, ProfileType profileType)
        {
            try
            {
                var profiles = new List<Profile>();
                using (var db = new Ri2Context())
                {
                    var gr = db.Groups.FirstOrDefault(x => x.Id == group.Id);
                    if (gr == null) return;
                    foreach (var ind in ids)
                        if (profileType == ProfileType.Rcf)
                        {
                            var pr = db.RcfProfiles.FirstOrDefault(x => x.RcfId == ind);
                            if (pr == null)
                            {
                                Logger.Log("ImportService", $"Rcf id {ind} not found", LogLevel.Warning);
                                continue;
                            }

                            if (db.Profiles.Any(x => x.RcfProfileId == pr.Id)) continue;
                            profiles.Add(new Profile {RcfProfile = pr, FideProfile = pr.FideProfile, Group = gr});
                        }
                        else
                        {
                            var pr = db.FideProfiles.FirstOrDefault(profile => profile.FideId == ind);
                            if (pr == null)
                            {
                                Logger.Log("ImportService", $"Fide id {ind} not found", LogLevel.Warning);
                                continue;
                            }

                            var rcf = db.RcfProfiles.FirstOrDefault(profile => profile.FideProfileId == pr.Id);
                            if (db.Profiles.Any(x => x.FideProfileId == pr.Id)) continue;
                            profiles.Add(new Profile {FideProfile = pr, RcfProfile = rcf, Group = gr});
                        }

                    db.Profiles.AddRange(profiles);
                    db.SaveChanges();
                    Logger.Log("ImportService", $"Imported {profiles.Count}");
                    //await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("ImportService", $"Error: {ex.Message}", LogLevel.Error);
            }
        }
    }
}