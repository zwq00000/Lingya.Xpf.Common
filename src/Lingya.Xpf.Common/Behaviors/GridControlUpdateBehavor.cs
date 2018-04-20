using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;

namespace Lingya.Xpf.Behaviors {
    /// <summary>
    /// 表格更新行为，<see cref="GridControl.BeginDataUpdate"/><see cref="GridControl.EndDataUpdate"/>
    /// </summary>
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