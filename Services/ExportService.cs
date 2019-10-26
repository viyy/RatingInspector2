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
            Logger.Log("ExportService", $"Registered {_plugins.Count} plugins");
        }

        public Task ExportAsync(IEnumerable<Group> groups, ExportSettings settings, Guid pluginGuid)
        {
            try
            {
                var enumerable = groups.ToList();
                Logger.Log("ExportService",
                    $"Export start for groups {string.Join(", ", enumerable.Select(g => g.Name))}");
                var t = new List<Profile>();
                using (var db = new Ri2Context())
                {
                    foreach (var g in enumerable)
                    {
                        var gr = db.Groups.Include(x => x.Profiles).Include(x => x.Profiles.Select(p => p.FideProfile))
                            .Include(x => x.Profiles.Select(p => p.RcfProfile).Select(r => r.FideProfile))
                            .Single(x => x.Id == g.Id);
                        t.AddRange(gr.Profiles);
                    }
                }

                //License Block++
                var limit = License.GetData("export");
                if (!int.TryParse(limit, out var limitResult))
                {
                    throw new InvalidLicenseException("Export: Invalid License Data");
                }

                if (limitResult == -1 || t.Count <= limitResult) return ExportAsync(t, settings, pluginGuid);
                Logger.Log("ExportService", "Limited export", LogLevel.Warning);
                t = t.Take(limitResult).ToList();
                //License Block--
                return ExportAsync(t, settings, pluginGuid);
            }
            catch (Exception e)
            {
                Logger.Log("ExportService", $"Error: {e.Message}", LogLevel.Error);
            }

            return Task.CompletedTask;
        }

        public Task ExportAsync(IEnumerable<Profile> profiles, ExportSettings settings, Guid pluginGuid)
        {
            try
            {
                var enumerable = profiles.ToList();
                Logger.Log("ExportService", $"Export profiles ({enumerable.Count()})");
                //License Block++
                var limit = License.GetData("export");
                if (!int.TryParse(limit, out var limitResult))
                {
                    throw new InvalidLicenseException("Export: Invalid License Data");
                }

                if (limitResult == -1 || enumerable.Count <= limitResult)
                    return _plugins.First(x => x.GUID == pluginGuid).ExportAsync(enumerable, settings);
                Logger.Log("ExportService", "Limited export", LogLevel.Warning);
                enumerable = enumerable.Take(limitResult).ToList();
                //License Block--
                return _plugins.First(x => x.GUID == pluginGuid).ExportAsync(enumerable, settings);
            }
            catch (Exception e)
            {
                Logger.Log("ExportService", $"Error: {e.Message}", LogLevel.Error);
            }
            return Task.CompletedTask;
        }

        public IEnumerable<IFileExporter> GetPlugins()
        {
            return _plugins;
        }
    }
}