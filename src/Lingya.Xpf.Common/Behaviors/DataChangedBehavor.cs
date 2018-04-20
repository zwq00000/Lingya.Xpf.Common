using System.Diagnostics;
using System.Windows;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using EventTrigger = DevExpress.Mvvm.UI.Interactivity.EventTrigger;

namespace Lingya.Xpf.Behaviors {
    /// <summary>
    /// 根据控件事件更新数据变化属性
    /// </summary>
    /// <example>
    /// <dxg:TableView x:Name="TableView">
    ///     <dxmvvm:Interaction.Behaviors>
    ///         <behaviors:DataChangedBehavor EventName="CellValueChanged" DataChanged="{Binding HasChanges,Mode=OneWayToSource}"/>
    ///     </Interaction.Behaviors>
    /// </TableView>
    /// </example>
    public class DataChangedBehavor : EventTrigger {
        /// <summary>
        ///                 <para>Identifies the  dependency property.
        /// </para>
        ///             </summary>
        /// <returns> </returns>
        public static readonly DependencyProperty HasDataChangedProperty = DependencyProperty.Register(nameof(DataChanged), typeof(bool),typeof(DataChangedBehavor), new PropertyMetadata(false));


        public bool DataChanged {
            get {
                return (bool)GetValue(HasDataChangedProperty);
            }
            set {
                SetValue(HasDataChangedProperty,value);
            }
        }

        protected override void OnEvent(object sender, object eventArgs) {
            Debug.WriteLine($"On Event {EventName} {eventArgs}");
            this.DataChanged = true;
        }
    }
}