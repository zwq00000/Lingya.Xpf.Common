using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;

namespace Lingya.Xpf.Behaviors {
    /// <summary>
    /// 表格更新行为，<see cref="GridControl.BeginDataUpdate"/><see cref="GridControl.EndDataUpdate"/>
    /// </summary>
    /// <remarks>
    ///  <a href="https://documentation.devexpress.com/WindowsForms/DevExpress.XtraGrid.Views.Base.ColumnView.BeginDataUpdate.method">
    /// ColumnView.BeginDataUpdate方法
    /// </a>
    /// 如果需要执行影响视图外观的一系列操作，则应该在调用BaseView.BeginUpdate和BaseView.EndUpdate方法之间包含代码。这些方法允许视图被锁定，以便在按顺序执行多个更改时不会更新视觉控件。但是，BeginUpdate和EndUpdate方法不会阻止内部数据更新。要防止内部数据更新，请使用BeginDataUpdate和BaseView.EndDataUpdate方法。在执行以下任何一项操作时，这些功能可以提高电网控制性能：
    ///     <li> 按列排序或分组;</li>
    ///     <li>数据排序时修改View的记录;</li>
    ///     <li>在数据源级别添加，删除或修改记录。</li>
    ///     如果使用BeginDataUpdate和EndDataUpdate方法对执行任何这些操作的序列执行的代码进行了包装，则View将仅执行一次数据更新，以反映在调用EndDataUpdate方法后所做的所有更改。
    ///     该BeginDataUpdate和EndDataUpdate方法调用的BeginUpdate和EndUpdate内部方法。这可以防止视图更新。
    ///     当网格控制绑定到实现IBindingList接口的数据源时。它自动订阅IBindingList.ListChanged事件。这使网格能够响应数据源级别所做的更改。该BeginDataUpdate方法暂时退订从这个事件网格控制。该EndDataUpdate方法被调用时，再次签约电网控制这个事件。不要在BeginDataUpdate和EndDataUpdate方法调用之间向数据源添加列，因为网格控件无法正确响应这些更改。此操作也可能引发异常。
    ///     另外，请勿在BeginDataUpdate和。之间重新分配数据源EndDataUpdate方法调用。
    /// </remarks>
    public class GridControlUpdateBehavor : Behavior<GridControl> {

        /// <summary>
        ///                 <para>Identifies the  dependency property.
        /// </para>
        ///             </summary>
        /// <returns> </returns>
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(GridControlUpdateBehavor), new PropertyMetadata(false, OnIsLoadingChanged));


        private static void OnIsLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != e.OldValue) {
                if (d is GridControlUpdateBehavor behavor) {
                    if ((bool) e.NewValue) {
                        behavor.AssociatedObject.BeginDataUpdate();
                    } else {
                        behavor.AssociatedObject.EndDataUpdate();
                    }
                }
            }
        }

        private void OnLoadingChanged(bool value) {
            if (value) {
                this.AssociatedObject.BeginDataUpdate();
            } else {
                this.AssociatedObject.EndDataUpdate();
            }
        }

        protected override void OnAttached() {
            base.OnAttached();

        }

        public bool IsLoading {
            get { return (bool)GetValue(IsLoadingProperty); }
            set {
                SetValue(IsLoadingProperty, value);
            }
        }
    }
}