using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Lingya.Xpf.Common {
    /// <summary>
    /// 数据查询日期参数
    /// </summary>
    public class DateRange : INotifyPropertyChanged {
        private int _duringDays;
        private DateTime _start;
        private DateTime _end;
        private DateTime _maxDate;
        private DateTime _minDate;

        private string _description;

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime Start {
            get { return _start; }
            set {
                if (value.Equals(_start)) return;
                _start = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DuringDays));
                this._description = $"{Start:d} - {End:d}";
            }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End {
            get { return _end; }
            set {
                if (value.Equals(_end)) return;
                _end = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DuringDays));
                this._description = $"{Start:d} - {End:d}";
            }
        }

        /// <summary>
        /// 查询天数
        /// </summary>
        public int DuringDays {
            get { return ( End - Start ).Days +1; }
            set {
                if (value == _duringDays) return;
                if (value < 0) {
                    value = 0;
                }
                _duringDays = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///日期范围开始
        /// </summary>
        public DateTime MinDate {
            get { return _minDate; }
            set {
                if (value.Equals(_minDate)) return;
                _minDate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 日期范围结束
        /// </summary>
        public DateTime MaxDate {
            get { return _maxDate; }
            set {
                if (value.Equals(_maxDate)) return;
                _maxDate = value;
                OnPropertyChanged();
            }
        }

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString() {
            return _description;
        }

        #endregion

        #region  Static Methods

        /// <summary>
        /// 当天
        /// </summary>
        public static DateRange Today => new DateRange().ToToday();

        /// <summary>
        /// 本周
        /// </summary>
        public static DateRange ThisWeek {
            get {
                return new DateRange().ToThisWeek();
            }
        }

        /// <summary>
        /// 本月
        /// </summary>
        public static DateRange ThisMonth {
            get { return new DateRange().ToThisMonth(); }
        }

        #endregion

        /// <summary>
        /// 今天
        /// </summary>
        /// <returns></returns>
        public DateRange ToToday() {
            Start = End = DateTime.Today;
            _description = Start.Date.ToShortDateString();
            return this;
        }

        /// <summary>
        /// 本周
        /// </summary>
        /// <returns></returns>
        public DateRange ToThisWeek() {
            var dayOfWeek = (int)DateTime.Today.DayOfWeek;
            Start = DateTime.Today.AddDays(-dayOfWeek);
            End = DateTime.Today.AddDays(7 - dayOfWeek);
            return this;
        }

        /// <summary>
        /// 本月
        /// </summary>
        /// <returns></returns>
        public DateRange ToThisMonth() {
            var thisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            Start = thisMonth;
            End = thisMonth.AddMonths(1).AddDays(-1);
            _description = $"{Start:yyyy年M月}";
            return this;
        }

        /// <summary>
        /// 最近 n 天
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public DateRange ToLastDays(int days) {
            End = DateTime.Today;
            Start = End.AddDays(-days);
            return this;
        }

        /// <summary>
        /// 开始日期
        /// </summary>
        /// <returns></returns>
        public DateTime GetStarDate() {
            if (Start > End) {
                return End.Date;
            } else {
                return Start.Date;
            }
        }

        /// <summary>
        /// 结束日期,比给出的结束日期+1天
        /// 查询条件 < EndDate
        /// </summary>
        /// <returns></returns>
        public DateTime GetEndDate() {
            if (Start > End) {
                return Start.Date.AddDays(1);
            } else {
                return End.Date.AddDays(1);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

#if NET40
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#else
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#endif
    }
}