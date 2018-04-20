using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Mvvm;
using JetBrains.Annotations;

namespace Lingya.Xpf.Common {
    /// <summary>
    /// 查询数据 RibbonGroup控制器
    /// </summary>
    public class QueryDateController:INotifyPropertyChanged{
        private bool _canLoad;

        public QueryDateController() {
            QueryDate = DateRange.Today;
            InitCommands();
        }

        private void InitCommands() {
            LoadData = new AsyncCommand(async () => {
                await Callback(QueryDate);
            }, CanLoad);

            LoadToday = new AsyncCommand(async () => {
                QueryDate.ToToday();
                await Callback(QueryDate);
            },CanLoad);

            LoadThisWeek = new AsyncCommand(async () => {
                QueryDate.ToThisWeek();
                await Callback(QueryDate);
            }, CanLoad);

            LoadThisMonth = new AsyncCommand(async () => {
                QueryDate.ToThisMonth();
                await Callback(QueryDate);
            }, CanLoad);

            LoadLastDays = new AsyncCommand<int>(async (days) => {
                QueryDate.ToLastDays(days);
                await Callback(QueryDate);
            }); 
        }

        public bool CanLoad {
            get { return _canLoad; }
            set {
                if (value == _canLoad) return;
                _canLoad = value;
                OnPropertyChanged();
            }
        }

        public Func<DateRange,Task> Callback { get; set; }

        public ICommand LoadData {
            get; private set;
        }

        public ICommand LoadToday {
            get; private set;
        }

        public ICommand LoadThisWeek {
            get; private set;
        }

        public ICommand LoadThisMonth { get; private set; }

        /// <summary>
        /// 加载最近n天数据
        /// </summary>
        public ICommand<int> LoadLastDays { get; private set; }

        /// <summary>
        /// 查询日期范围
        /// </summary>
        public DateRange QueryDate { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}