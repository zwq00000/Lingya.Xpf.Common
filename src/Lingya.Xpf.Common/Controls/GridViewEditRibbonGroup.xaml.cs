using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Printing;
using Lingya.Xpf.Behaviors;

namespace Lingya.Xpf.Controls {
    /// <summary>
    ///     GridViewEditRibbonGroup.xaml 的交互逻辑
    /// </summary>
    public partial class GridViewEditRibbonGroup {
        /// <summary>
        ///     <para>
        ///         Identifies the  dependency property.
        ///     </para>
        /// </summary>
        /// <returns> </returns>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(IPrintableControl), typeof(GridViewEditRibbonGroup));


        /// <summary>
        ///     标题属性
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(GridViewEditRibbonGroup));


        public GridViewEditRibbonGroup() {
            InitializeComponent();
        }

        /// <summary>
        ///     是否允许新增
        /// </summary>
        [DefaultValue(false)]
        public bool AllowAddNew {
            get => this.AddNewButton.IsVisible;
            set => this.AddNewButton.IsVisible = value;
        }

        /// <summary>
        /// 是否允许删除按钮
        /// </summary>
        [DefaultValue(false)]
        public bool AllowDelete {
            get => this.DeleteButton.IsVisible;
            set => this.DeleteButton.IsVisible = value;
        }

        /// <summary>
        /// 是否显示编辑按钮
        /// </summary>
        [DefaultValue(false)]
        public bool AllowEdit {
            get => this.EditButton.IsVisible;
            set => this.EditButton.IsVisible = value;
        }

        /// <summary>
        /// 是否显示导出按钮
        /// </summary>
        [DefaultValue(true)]
        public bool AllowExport {
            get => this.ExportButton.IsVisible;
            set => this.ExportButton.IsVisible = value;
        }

        /// <summary>
        /// 是否显示保存按钮
        /// </summary>
        [DefaultValue(true)]
        public bool AllowSave {
            get => this.SaveButton.IsVisible;
            set => this.SaveButton.IsVisible = value;
        }

        /// <summary>
        ///     <para>
        ///         Gets or sets the source-object. This is a dependency property.
        ///     </para>
        /// </summary>
        /// <value>
        ///     <para>An object specifying the source-object.</para>
        ///     <para>The default is the DataContext of the <see cref="P:DevExpress.Mvvm.UI.FunctionBindingBehaviorBase.Target" />.</para>
        /// </value>
        public IPrintableControl Source {
            get => (IPrintableControl) GetValue(SourceProperty);
            set {
                SetValue(SourceProperty, value);
                ExportButton.IsEnabled = value != null;
            }
        }

        /// <summary>
        ///     导出文件标题
        /// </summary>
        public string Title {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        private void BarItem_OnItemClick(object sender, ItemClickEventArgs e) {
            if (Source != null) Source.Export(Title);
        }
    }
}