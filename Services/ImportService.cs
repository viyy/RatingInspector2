using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using DAL;
using Interfaces;
using Models;
using PluginShared;
using Services.Helpers;

namespace Services
{
    public class ImportService : BaseGroupListProvider, IImportService
    {
        private readonly List<IFileImporter> _plugins = new List<IFileImporter>();

        public ImportService()
        {
            _plugins.AddRange(PluginManager.GetPlugins<IFileImporter>());
        }

        public string GetFilters()
        {
            return string.Join("|", _plugins.Select(x => x.Filter));
        }

        public IEnumerable<int> LoadFromFile(string path)
        {
            var plugin = _plugins.FirstOrDefault(x => x.SupportedFormats.Any(path.EndsWith));
            return plugin == null ? new List<int>() : plugin.Read(path);
        }

        public void ImportAsync(IEnumerable<int> ids, Group group, ProfileType profileType)
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
                            File.AppendAllText("err.log",
                                DateTime.Now.ToShortTimeString() + "|ImportService|RcfId" + ind + "not found" +
                                Environment.NewLine);
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
                            File.AppendAllText("err.log",
                                DateTime.Now.ToShortTimeString() + "|ImportService|FideId " + ind + "not found" +
                                Environment.NewLine);
                            continue;
                        }

                        var rcf = db.RcfProfiles.FirstOrDefault(profile => profile.FideProfileId == pr.Id);
                        if (db.Profiles.Any(x => x.FideProfileId == pr.Id)) continue;
                        profiles.Add(new Profile {FideProfile = pr, RcfProfile = rcf, Group = gr});
                    }

                db.Profiles.AddRange(profiles);
                db.SaveChanges();
                //await db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}