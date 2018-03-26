using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Charts;
using Lingya.Xpf.Behaviors;

namespace ASCP.Behaviors {
    public class ChartCotextBehavior : Behavior<ChartControl> {
        /// <summary>
        /// 图表标题 依赖属性
        /// </summary>
        public static readonly DependencyProperty ChartTitleProperty = DependencyProperty.Register(nameof(ChartTitle),
            typeof(string), typeof(ChartCotextBehavior), new PropertyMetadata(string.Empty,OnTitlePropertyChanged));

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        }

        public string ChartTitle {
            get { return (string)this.GetValue(ChartTitleProperty); }
            set {SetValue(ChartTitleProperty, value);
            }
        }

        protected override void OnAttached() {
            this.AssociatedObject.ContextMenuOpening += AssociatedObject_ContextMenuOpening;
        }

        protected override void OnDetaching() {
            this.AssociatedObject.ContextMenuOpening -= AssociatedObject_ContextMenuOpening;
        }

        private void AssociatedObject_ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e) {
            CustomerChartCommands.SetViewSource(this.AssociatedObject);
            CustomerChartCommands.Title = this.ChartTitle;
        }
    }
}