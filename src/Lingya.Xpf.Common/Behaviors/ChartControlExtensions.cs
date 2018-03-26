using System;
using System.IO;
using System.Windows;
using DevExpress.XtraPrinting;
using Microsoft.Win32;

namespace Lingya.Xpf.Behaviors {

    public static class ChartControlExtensions {
        
        /// <summary>
        /// 图表控件导出到文件
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="fileName"></param>
        public static void Export(this ChartControl chart, string fileName) {
            const string mutiFileFilter = "Excel 文件(*.xlsx)|*.xlsx|PDF 文件 (*.pdf)|*.pdf|Xps 文件(*.xps)|*.xps|Jpeg 文件(*.jpeg)|*.jpeg";
            fileName = fileName.Replace('<', '[').Replace('>', ']');

            var dialog = new SaveFileDialog() {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = $".csv",
                Filter = mutiFileFilter,
                FileName = fileName
            };
            var result = dialog.ShowDialog(Application.Current.MainWindow);
            if (result.Value) {
                using (var stream = dialog.OpenFile()) {
                    var extension = Path.GetExtension(dialog.FileName);
                    switch (dialog.FilterIndex) {
                        case 1:
                            chart.ExportToXlsx(stream, PrintSizeMode.Stretch);
                            break;
                        case 2:
                            chart.ExportToPdf(stream, PrintSizeMode.Stretch);
                            break;
                        case 3:
                            chart.ExportToXps(stream, PrintSizeMode.Stretch);
                            break;
                        case 4:
                            chart.ExportToXps(stream, PrintSizeMode.Stretch);
                            break;
                        case 5:
                            chart.ExportToImage(stream, new ImageExportOptions() {
                                ExportMode = ImageExportMode.SingleFilePageByPage,
                                Format = ImageFormat.Jpeg
                            });
                            break;
                        default:
                            throw new NotSupportedException($"不支持的文件导出格式 {extension}");
                    }

                }
            }
        }

        /// <summary>
        /// 复制到剪贴板
        /// </summary>
        /// <param name="chart"></param>
        public static void CopyToClipboard(this ChartControl chart) {
            using (var stream = new MemoryStream()) {
                chart.ExportToImage(stream);
                var image = System.Drawing.Image.FromStream(stream);
                System.Windows.Forms.Clipboard.SetImage(image);
            }
        }

        /// <summary>
        /// 图表控件内容导出图片
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="fileName"></param>
        public static void ExportImage(this ChartControl chart, string fileName) {
            const string mutiFileFilter = "Jpeg 文件(*.jpeg)|*.jpeg|Png 文件(*.png)|*.png|Bmp 文件(*.bmp)|*.bmp";
            fileName = fileName.Replace('<', '[').Replace('>', ']');

            var dialog = new SaveFileDialog() {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = $".csv",
                Filter = mutiFileFilter,
                FileName = fileName
            };
            var result = dialog.ShowDialog(Application.Current.MainWindow);
            if (result.Value) {
                using (var stream = dialog.OpenFile()) {
                    var extension = Path.GetExtension(dialog.FileName);
                    switch (dialog.FilterIndex) {
                        case 1:
                            chart.ExportToImage(stream, new ImageExportOptions(ImageFormat.Jpeg), PrintSizeMode.Stretch);
                            break;
                        case 2:
                            chart.ExportToImage(stream, new ImageExportOptions(ImageFormat.Png), PrintSizeMode.Stretch);
                            break;
                        case 3:
                            chart.ExportToImage(stream, new ImageExportOptions(ImageFormat.Bmp), PrintSizeMode.Stretch);
                            break;
                        default:
                            throw new NotSupportedException($"不支持的文件导出格式 {extension}");
                    }
                }
            }
        }
    }
}