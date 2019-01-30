using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task ImportAsync(IEnumerable<int> ids, Group group, ProfileType profileType)
        {
            var profiles = new List<Profile>();
            using (var db = new Ri2Context())
            {
                if (profileType == ProfileType.Rcf)
                    foreach (var id in ids)
                    {
                        var pr = db.RcfProfiles.FirstOrDefault(x => x.RcfId == id);
                        if (pr == null)
                        {
                            File.AppendAllText("err.log",
                                DateTime.Now.ToShortTimeString() + "|ImportService|RcfId" + id + "not found\n");
                            continue;
                        }

                        if (db.Profiles.Any(x => x.RcfProfileId == pr.Id)) continue;
                        profiles.Add(new Profile {RcfProfile = pr, FideProfile = pr.FideProfile, Group = group});
                    }
                else
                    foreach (var id in ids)
                    {
                        var pr = db.FideProfiles.SingleOrDefault(x => x.FideId == id);
                        if (pr == null)
                        {
                            File.AppendAllText("err.log",
                                DateTime.Now.ToShortTimeString() + "|ImportService|FideId " + id + "not found\n");
                            continue;
                        }

                        var rcf = db.RcfProfiles.SingleOrDefault(x => x.FideProfile == pr);
                        if (db.Profiles.Any(x => x.FideProfile == pr)) continue;
                        profiles.Add(new Profile {FideProfile = pr, RcfProfile = rcf, Group = group});
                    }

                db.Profiles.AddRange(profiles);
                await db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}