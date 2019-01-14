using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Interfaces;
using Models;

namespace PluginShared
{
    public interface IFileExporter : IPlugin
    {
        string Shortcut { get; }
        string Filter { get; }
        Task ExportAsync(IEnumerable<Profile> profiles, ExportSettings settings);
    }
}