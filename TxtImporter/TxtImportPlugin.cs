using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PluginShared;

namespace TxtImporter
{
    public class TxtImportPlugin : IFileImporter
    {
        public Guid GUID => Guid.Parse("7f54fd28-fd23-4833-b92e-0f2f057f075c");
        public string Name => "Text File Importer";
        public string Author => "Nelfias";
        public int Version => 1;
        public string Description => "Import list of id from txt file";
        public string Filter => "*.txt|*.txt";
        public IEnumerable<string> SupportedFormats => new[] {"txt"};

        public bool TryRead(string path, out IEnumerable<int> list)
        {
            list = new List<int>();
            var tmp = new List<int>();
            try
            {
                if (!File.Exists(path)) return false;
                var strs = File.ReadAllLines(path);
                tmp.AddRange(strs.Select(int.Parse));
                list = tmp;
            }
            catch (Exception e)
            {
                File.AppendAllText("err.log", DateTime.Now.ToShortTimeString() + "|TxtImporter|" + e.Message + "\n");
                return false;
            }

            return true;
        }

        public IEnumerable<int> Read(string path)
        {
            var list = new List<int>();
            try
            {
                if (!File.Exists(path)) return list;
                var strs = File.ReadAllLines(path);
                list.AddRange(strs.Select(int.Parse));
            }
            catch (Exception e)
            {
                File.AppendAllText("err.log", DateTime.Now.ToShortTimeString() + "|TxtImporter|" + e.Message);
            }

            return list;
        }
    }
}