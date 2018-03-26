using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using DevExpress.Xpf.Printing;
using Microsoft.Win32;

namespace Lingya.Xpf.Behaviors {
    internal static class ExportHelper {
        const string MutiFileFilter = "CSV 文件(*.csv)|*.csv|Excel 文件(*.xlsx)|*.xlsx|PDF 文件 (*.pdf)|*.pdf";

        public static void Export(this IPrintableControl printable, string fileName) {

            fileName = fileName.Replace('<', '[').Replace('>', ']');

            var dialog = new SaveFileDialog() {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = $".csv",
                Filter = MutiFileFilter,
                FileName = fileName
            };
            var result = dialog.ShowDialog(Application.Current.MainWindow);
            if (result.Value) {
                using (var stream = dialog.OpenFile()) {
                    var extension = Path.GetExtension(dialog.FileName);
                    switch (dialog.FilterIndex) {
                        case 1:
                            PrintHelper.ExportToCsv(printable, stream);
                            break;
                        case 2:
                            PrintHelper.ExportToXlsx(printable, stream);
                            break;
                        case 3:
                            PrintHelper.ExportToPdf(printable, stream);
                            break;
                        default:
                            throw new NotSupportedException($"不支持的文件导出格式 {extension}");
                    }
                }
                OpenFile(dialog.FileName);
            }
        }

        private static void OpenFile(string path, bool showDialog = true) {
            if (File.Exists(path)) {
                if (showDialog) {
                    var fileName = Path.GetFileName(path);
                    var result = MessageBox.Show($"是否打开文件{fileName}?", "提示信息", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) {
                        return;
                    }
                }
                Process.Start(path);
            }
        }
    }
}