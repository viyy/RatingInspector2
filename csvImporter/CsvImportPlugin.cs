using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PluginShared;

namespace CsvImporter
{
    public class CsvImportPlugin : IFileImporter
    {
        // {2EFC3746-150A-4266-8150-F57C95CC1073}
        public Guid GUID => Guid.Parse("2EFC3746-150A-4266-8150-F57C95CC1073");
        public string Name => "CsvFileImporter";
        public string Author => "Nelfias";
        public int Version => 1;
        public string Description => "Import list of id from csv file";
        public string Filter => "*.csv|*.csv";
        public IEnumerable<string> SupportedFormats => new[] {"csv"};

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
                File.AppendAllText("err.log", DateTime.Now.ToShortTimeString() + "|CSVImporter|" + e.Message + "\n");
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
                File.AppendAllText("err.log", DateTime.Now.ToShortTimeString() + "|CSVImporter|" + e.Message);
            }

            return list;
        }
    }
}