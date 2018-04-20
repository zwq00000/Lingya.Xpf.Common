using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Printing;
using Lingya.Xpf.Behaviors;

namespace Lingya.Xpf.Controls {
    /// <summary>
    ///     GridViewEditRibbonGroup.xaml 的交互逻辑
    /// </summary>
    public partial class OrderOperateRibbonGroup {

        public OrderOperateRibbonGroup() {
            InitializeComponent();
        }

        /// <summary>
        ///     是否允许新增
        /// </summary>
        [DefaultValue(false)]
        public bool AllowCancelAudit {
            get => this.CancelAuditButton.IsVisible;
            set => this.CancelAuditButton.IsVisible = value;
        }

        /// <summary>
        /// 是否允许删除按钮
        /// </summary>
        [DefaultValue(false)]
        public bool AllowAudit {
            get => this.AuditButton.IsVisible;
            set => this.AuditButton.IsVisible = value;
        }

        /// <summary>
        /// 是否显示编辑按钮
        /// </summary>
        [DefaultValue(false)]
        public bool AllowAuditAll {
            get => this.AuditAllButton.IsVisible;
            set => this.AuditAllButton.IsVisible = value;
        }


    }
}