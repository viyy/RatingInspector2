using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DAL;
using Interfaces;
using Models;
using Nelfias.License;
using PluginShared;
using Services.Helpers;

namespace Services
{
    public class ExportService : BaseGroupListProvider, IExportService
    {
        private readonly List<IFileExporter> _plugins;

        public ExportService()
        {
            _plugins = PluginManager.GetPlugins<IFileExporter>();
        }

        public Task ExportAsync(IEnumerable<Group> groups, ExportSettings settings, Guid pluginGuid)
        {
            var t = new List<Profile>();
            using (var db = new Ri2Context())
            {
                foreach (var g in groups)
                {
                    var gr = db.Groups.Include(x => x.Profiles).Include(x => x.Profiles.Select(p => p.FideProfile))
                        .Include(x => x.Profiles.Select(p => p.RcfProfile).Select(r => r.FideProfile))
                        .Single(x => x.Id == g.Id);
                    t.AddRange(gr.Profiles);
                }
            }
            //License Block++
            var limit = License.GetData("export");
            if (!int.TryParse(limit, out var limitResult)) throw new InvalidLicenseException("Export: Invalid License Data");
            if (limitResult != -1 && t.Count>limitResult)
            {
                t = t.Take(limitResult).ToList();
            }
            //License Block--
            return ExportAsync(t, settings, pluginGuid);
        }

        public Task ExportAsync(IEnumerable<Profile> profiles, ExportSettings settings, Guid pluginGuid)
        {
            //License Block++
            var limit = License.GetData("export");
            if (!int.TryParse(limit, out var limitResult)) throw new InvalidLicenseException("Export: Invalid License Data");
            var enumerable = profiles.ToList();
            if (limitResult != -1 && enumerable.Count > limitResult)
            {
                enumerable = enumerable.Take(limitResult).ToList();
            }
            //License Block--
            return _plugins.First(x => x.GUID == pluginGuid).ExportAsync(enumerable, settings);
        }

        public IEnumerable<IFileExporter> GetPlugins()
        {
            return _plugins;
        }
    }
}