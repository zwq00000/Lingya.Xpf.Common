using System;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;

namespace Lingya.Xpf.Behaviors
{
    /// <summary>
    /// 自定义 UICommands
    /// </summary>
    public static class CustomerChartCommands
    {
        private static readonly WeakReference<ChartControl> ViewSourceReference = new WeakReference<ChartControl>(null);

        public static string Title { get; set; }

        public static void SetViewSource(ChartControl view)
        {
            ViewSourceReference.SetTarget(view);
        }

        private static ChartControl GetView()
        {
            ChartControl view = null;
            ViewSourceReference.TryGetTarget(out view);
            return view;
        }

        public static ICommand Copy => new DelegateCommand(() =>
        {
            var chart = GetView();
            chart?.CopyToClipboard();
            chart?.ClearValue(FrameworkElement.DataContextProperty);
        });

        public static ICommand ShowPrintPreview => new DelegateCommand(() =>
        {
            var chart = GetView();
            chart?.ShowPrintPreview(chart);
        });

        public static ICommand Export => new DelegateCommand(() =>
        {
            var chart = GetView();
            chart?.Export(Title);
            chart?.ClearValue(FrameworkElement.DataContextProperty);
        });

        public static ICommand ExportToImage => new DelegateCommand(() =>
        {
            var chart = GetView();
            chart?.ExportImage(Title);
            chart?.ClearValue(FrameworkElement.DataContextProperty);
        });

        public static ICommand PrintPreview => new DelegateCommand(() =>{
            var chart = GetView();
            chart?.ShowPrintPreview(Application.Current.MainWindow, Title, PrintSizeMode.Stretch);
            chart?.ClearValue(FrameworkElement.DataContextProperty);
        });

    }
}