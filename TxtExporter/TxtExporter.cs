using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common;
using Models;
using PluginShared;

namespace TxtExporter
{
    public class TxtExporter : IFileExporter
    {
        public Guid GUID => Guid.Parse("a0e31a93-1c03-4c64-ae38-dd0b175395d7");
        public string Name => "TxtExporter";
        public string Author => "Nelfias";
        public int Version => 1;
        public string Description => "Export to simple txt file";
        public string Shortcut => "TXT";
        public string Filter => "*.txt|*.txt";
        public async Task ExportAsync(IEnumerable<Profile> profiles, ExportSettings settings)
        {
            foreach (var profile in profiles)
            {
                var str = "";
                if (settings.Rcf)
                    str += profile.RcfProfile?.RcfId + "\t";
                if (settings.Fide)
                    str += profile.FideProfile?.FideId + "\t";
                if (settings.RuName)
                {
                    if (profile.RcfProfile != null)
                        str += profile.RcfProfile.Name + "\t";
                    else
                    {
                        str += profile.FideProfile?.Name + "\t";
                    }
                }

                if (settings.EngName)
                {
                    if (profile.FideProfile != null)
                        str += profile.FideProfile.Name+"\t";
                    else
                    {
                        str += "\t";
                    }
                }

                if (settings.Birth)
                    str += (profile.RcfProfile?.Birth ?? profile.FideProfile?.Birth) + "\t";
                if (settings.RcfRat[0])
                    str += (profile.RcfProfile?.Std.ToString() ?? "0") + "\t";
                if (settings.RcfRat[1])
                    str += (profile.RcfProfile?.Rpd.ToString() ?? "0") + "\t";
                if (settings.RcfRat[2])
                    str += (profile.RcfProfile?.Blz.ToString() ?? "0") + "\t";
                if (settings.FideRat[0])
                    str += (profile.FideProfile?.Std.ToString() ?? "0") + "\t";
                if (settings.FideRat[1])
                    str += (profile.FideProfile?.Rpd.ToString() ?? "0") + "\t";
                if (settings.FideRat[2])
                    str += (profile.FideProfile?.Blz.ToString() ?? "0") + "\t";
                str += "\r\n";
                await Task.Run(() => File.AppendAllText("test.txt", str)).ConfigureAwait(false);
            }
        }
    }
}
