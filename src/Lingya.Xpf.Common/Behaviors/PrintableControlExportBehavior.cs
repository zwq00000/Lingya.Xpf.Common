using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;

namespace Lingya.Xpf.Behaviors {
    /// <summary>
    /// 可打印对象导出行为
    /// </summary>
    public class PrintableControlExportBehavior : Behavior<DependencyObject> {
        private const string UnknowTitle = "";

        /// <summary>
        ///                 <para>Identifies the  dependency property.
        /// </para>
        ///             </summary>
        /// <returns> </returns>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(IPrintableControl), typeof(PrintableControlExportBehavior), new PropertyMetadata(null, (d, e) => ((PrintableControlExportBehavior)d).OnSourceChanged()));

        /// <summary>
        /// 标题属性
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PrintableControlExportBehavior));

        public PrintableControlExportBehavior() {
            ResultCommand = new DelegateCommand<string>(ExportTo, false);
        }

        /// <summary>
        /// 导出文档
        /// </summary>
        private void ExportTo(string docName) {
            docName = GetTitle(docName);
            if (Source != null) {
                Source.Export(docName);
            } else {
                Debug.WriteLine($"{nameof(PrintableControlExportBehavior)}.Source is null ");
            }
        }

        private string GetTitle(string title) {
            title = Title ?? title;
            if (string.IsNullOrEmpty(title)) {
                return UnknowTitle;
            }
            return title.Replace("*", string.Empty).Replace(" ", string.Empty);
        }
        

        protected ICommand ResultCommand { get; private set; }

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


        protected object ActualSource { get; private set; }


        protected override void OnAttached() {
            base.OnAttached();
            (AssociatedObject as FrameworkElement).Do(x => x.DataContextChanged += OnAssociatedObjectDataContextChanged);
            (AssociatedObject as FrameworkContentElement).Do(x => x.DataContextChanged += OnAssociatedObjectDataContextChanged);
            UpdateActualSource();

            SetTargetProperty(AssociatedObject, "Command", ResultCommand);
        }


        protected override void OnDetaching() {
            (AssociatedObject as FrameworkElement).Do(x => x.DataContextChanged -= OnAssociatedObjectDataContextChanged);
            (AssociatedObject as FrameworkContentElement).Do(x => x.DataContextChanged -= OnAssociatedObjectDataContextChanged);
            (ActualSource as INotifyPropertyChanged).Do(x => x.PropertyChanged -= OnSourceObjectPropertyChanged);
            ActualSource = null;
            base.OnDetaching();

        }

        protected virtual void OnTargetChanged(DependencyPropertyChangedEventArgs e) {
            if (e.NewValue == e.OldValue)
                return;
            OnResultAffectedPropertyChanged();
        }

        protected virtual void OnResultAffectedPropertyChanged() {
        }

        protected virtual void UpdateActualSource() {
            (ActualSource as INotifyPropertyChanged).Do(x => x.PropertyChanged -= OnSourceObjectPropertyChanged);
            ActualSource = Source ?? GetAssociatedObjectDataContext();
            (ActualSource as INotifyPropertyChanged).Do(x => x.PropertyChanged += OnSourceObjectPropertyChanged);
            OnResultAffectedPropertyChanged();
        }

        private object GetAssociatedObjectDataContext() {
            return (AssociatedObject as FrameworkElement).With(x => x.DataContext) ?? (AssociatedObject as FrameworkContentElement).With(x => x.DataContext);
        }

        private void OnSourceObjectPropertyChanged(object sender, PropertyChangedEventArgs e) {
            OnResultAffectedPropertyChanged();
        }

        private void OnAssociatedObjectDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (Source != null)
                return;
            UpdateActualSource();
        }

        private void OnSourceChanged() {
            UpdateActualSource();
        }

        private void SetTargetProperty(object target, string property, object value) {
            if (target == null || !IsAttached || string.IsNullOrEmpty(property))
                return;

            DependencyProperty targetDependencyProperty = ObjectPropertyHelper.GetDependencyProperty(AssociatedObject, property);
            if (targetDependencyProperty != null) {
                ((DependencyObject)target).SetValue(targetDependencyProperty, value);
            }
        }
    }
}