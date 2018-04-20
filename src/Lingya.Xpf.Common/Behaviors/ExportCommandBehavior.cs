using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Printing;

namespace Lingya.Xpf.Behaviors {
    /// <summary>
    /// 表格数据 导出命令 <see cref="ApplicationCommands.SaveAs"/>
    /// </summary>
    public class ExportCommandBehavior : Behavior<UIElement> {

        /// <summary>
        ///                 <para>Identifies the  dependency property.
        /// </para>
        ///             </summary>
        /// <returns> </returns>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(IPrintableControl), typeof(ExportCommandBehavior));

        /// <summary>
        /// 标题属性
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ExportCommandBehavior));

        protected override void OnAttached() {
            base.OnAttached();
            this.AssociatedObject.CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, OnExecutedRouted));
        }

        private void OnExecutedRouted(object sender, ExecutedRoutedEventArgs e) {
            if (!e.Handled) {
                ExportTo(Title);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 导出文档
        /// </summary>
        private void ExportTo(string docName) {
            if (Source != null) {
                Source.Export(docName);
            } else {
                Debug.WriteLine($"{nameof(PrintableControlExportBehavior)}.Source is null ");
            }
        }

        /// <summary>
        ///                 <para>Gets or sets the source-object. This is a dependency property.
        /// </para>
        ///             </summary>
        /// <value><para>An object specifying the source-object.</para>
        /// <para>The default is the DataContext of the <see cref="P:DevExpress.Mvvm.UI.FunctionBindingBehaviorBase.Target" />.</para>
        /// 
        /// </value>
        public IPrintableControl Source {
            get {
                return (IPrintableControl)GetValue(SourceProperty);
            }
            set {
                SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// 导出文件标题
        /// </summary>
        public string Title {
            get {
                return (string)GetValue(TitleProperty);
            }
            set {
                SetValue(TitleProperty, value);
            }
        }
    }
}