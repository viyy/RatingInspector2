using System.Collections.Generic;
using Interfaces;

namespace PluginShared
{
    public interface IFileImporter : IPlugin
    {
        string Filter { get;}
        IEnumerable<string> SupportedFormats { get; }
        bool TryRead(string path, out IEnumerable<int> list);
        IEnumerable<int> Read(string path);
    }
}