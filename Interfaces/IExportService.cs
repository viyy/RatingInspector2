using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Models;
using PluginShared;

namespace Interfaces
{
    public interface IExportService : IGroupListProvider
    {
        Task ExportAsync(IEnumerable<Profile> profiles, ExportSettings settings, Guid pluginGuid);
        IEnumerable<IFileExporter> GetPlugins();
    }
}