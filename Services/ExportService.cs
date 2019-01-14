using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DAL;
using Interfaces;
using Models;
using PluginShared;

namespace Services
{
    public class ExportService : IExportService
    {
        public IEnumerable<Group> GetGroups()
        {
            using (var db = new Ri2Context())
            {
                return db.Groups.ToList();
            }
        }

        public Task ExportAsync(IEnumerable<Profile> profiles, ExportSettings settings, Guid pluginGuid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileExporter> GetPlugins()
        {
            throw new NotImplementedException();
        }
    }
}