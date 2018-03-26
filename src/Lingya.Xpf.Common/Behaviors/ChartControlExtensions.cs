using System;
using System.IO;
using System.Windows;
using DevExpress.XtraPrinting;
using Microsoft.Win32;

namespace Lingya.Xpf.Behaviors {

    public static class ChartControlExtensions {
        
        /// <summary>
        /// ͼ��ؼ��������ļ�
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="fileName"></param>
        public static void Export(this ChartControl chart, string fileName) {
            const string mutiFileFilter = "Excel �ļ�(*.xlsx)|*.xlsx|PDF �ļ� (*.pdf)|*.pdf|Xps �ļ�(*.xps)|*.xps|Jpeg �ļ�(*.jpeg)|*.jpeg";
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
                            throw new NotSupportedException($"��֧�ֵ��ļ�������ʽ {extension}");
                    }

                }
            }
        }

        /// <summary>
        /// ���Ƶ�������
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
        /// ͼ��ؼ����ݵ���ͼƬ
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="fileName"></param>
        public static void ExportImage(this ChartControl chart, string fileName) {
            const string mutiFileFilter = "Jpeg �ļ�(*.jpeg)|*.jpeg|Png �ļ�(*.png)|*.png|Bmp �ļ�(*.bmp)|*.bmp";
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
                            throw new NotSupportedException($"��֧�ֵ��ļ�������ʽ {extension}");
                    }
                }
            }
        }
    }
}