using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Common;
using Microsoft.Win32;
using Models;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using PluginShared;

namespace XslxExporter
{
    public class XslxExporter : IFileExporter
    {
        public Guid GUID => Guid.Parse("deb81394-db57-4960-be46-c7720ec30f1e");
        public string Name => "Excel Exporter";
        public string Author => "Nelfias";
        public int Version => 1;
        public string Description => "Export profiles to excel file";
        public string Shortcut => "XLSX";
        public string Filter => "*.xlsx|*.xlsx";

        public async Task ExportAsync(IEnumerable<Profile> profiles, ExportSettings settings)
        {
            await Task.Run(() => Export(profiles, settings)).ConfigureAwait(false);
        }

        private void MakeHeaders(ExcelWorksheet ws, ExportSettings s)
        {
            const int row = 1;
            var col = 1;
            if (s.Rcf)
            {
                ws.Cells[row, col].Value = "РШФ Id";
                col++;
            }

            if (s.Fide)
            {
                ws.Cells[row, col].Value = "Fide Id";
                col++;
            }

            if (s.RuName && s.EngName)
            {
                ws.Cells[row, col].Value = "Имя (Ру)";
                col++;
                ws.Cells[row, col].Value = "Имя (En)";
                col++;
            }
            else
            {
                ws.Cells[row, col].Value = "Имя";
                col++;
            }

            if (s.Birth)
            {
                ws.Cells[row, col].Value = "Год рожд.";
                col++;
            }

            if (s.RcfRat[0])
            {
                ws.Cells[row, col].Value = "РШФ Классика";
                col++;
            }

            if (s.RcfRat[1])
            {
                ws.Cells[row, col].Value = "РШФ Рапид";
                col++;
            }

            if (s.RcfRat[2])
            {
                ws.Cells[row, col].Value = "РШФ Блиц";
                col++;
            }

            if (s.FideRat[0])
            {
                ws.Cells[row, col].Value = "Fide Классика";
                col++;
            }

            if (s.FideRat[1])
            {
                ws.Cells[row, col].Value = "Fide Рапид";
                col++;
            }

            if (s.FideRat[2])
            {
                ws.Cells[row, col].Value = "Fide Блиц";
                col++;
            }

            if (s.Groups) ws.Cells[row, col].Value = "Группа";
        }

        private void WriteRow(ExcelWorksheet ws, Profile pr, ExportSettings s, int row)
        {
            var col = 1;
            if (s.Rcf)
            {
                ws.Cells[row, col].Value = pr.RcfProfile?.RcfId ?? 0;
                col++;
            }

            if (s.Fide)
            {
                ws.Cells[row, col].Value = pr.FideProfile?.FideId ?? 0;
                col++;
            }

            if (s.RuName && s.EngName)
            {
                ws.Cells[row, col].Value = pr.RcfProfile?.Name ?? "";
                col++;
                ws.Cells[row, col].Value = pr.FideProfile?.Name ?? "";
                col++;
            }
            else
            {
                string name;
                if (s.RuName)
                    name = pr.RcfProfile?.Name ?? pr.FideProfile?.Name ?? "";
                else
                    name = pr.FideProfile?.Name ?? pr.RcfProfile?.Name ?? "";
                ws.Cells[row, col].Value = name;
                col++;
            }

            if (s.Birth)
            {
                ws.Cells[row, col].Value = pr.RcfProfile?.Birth ?? pr.FideProfile?.Birth ?? 0;
                col++;
            }

            if (s.RcfRat[0])
            {
                ws.Cells[row, col].Value = pr.RcfProfile?.Std ?? 0;
                col++;
            }

            if (s.RcfRat[1])
            {
                ws.Cells[row, col].Value = pr.RcfProfile?.Rpd ?? 0;
                col++;
            }

            if (s.RcfRat[2])
            {
                ws.Cells[row, col].Value = pr.RcfProfile?.Blz ?? 0;
                col++;
            }

            if (s.FideRat[0])
            {
                ws.Cells[row, col].Value = pr.FideProfile?.Std ?? 0;
                col++;
            }

            if (s.FideRat[1])
            {
                ws.Cells[row, col].Value = pr.FideProfile?.Rpd ?? 0;
                col++;
            }

            if (s.FideRat[2])
            {
                ws.Cells[row, col].Value = pr.FideProfile?.Blz ?? 0;
                col++;
            }

            if (s.Groups) ws.Cells[row, col].Value = pr.Group.Name;
        }

        private void Export(IEnumerable<Profile> profiles, ExportSettings settings)
        {
            var dfn = "Rtg_" + DateTime.Now.ToShortDateString() + ".xlsx";
            var dlg = new SaveFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = Filter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = dfn,
                OverwritePrompt = true
            };
            if (dlg.ShowDialog() != true) return;
            var path = dlg.FileName;
            if (File.Exists(path)) File.Delete(path);
            var xlsx = new ExcelPackage(new FileInfo(path));
            var ws = xlsx.Workbook.Worksheets.Add("RTG-" + DateTime.Today.Day + "-" + DateTime.Today.Month + "-" +
                                                  DateTime.Today.Year);
            var row = 2;
            MakeHeaders(ws, settings);
            foreach (var profile in profiles)
            {
                WriteRow(ws, profile, settings, row);
                row++;
            }

            var range = ws.Cells[ws.Dimension.Start.Row, ws.Dimension.Start.Column, ws.Dimension.End.Row,
                ws.Dimension.End.Column];
            var table = ws.Tables.Add(range, "table1");
            range.AutoFitColumns();
            table.TableStyle = TableStyles.Light1;
            xlsx.Save();
            Process.Start(path);
        }
    }
}