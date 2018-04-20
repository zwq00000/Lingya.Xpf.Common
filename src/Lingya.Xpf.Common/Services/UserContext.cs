using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using DevExpress.Data.Async;

namespace Lingya.Xpf.Services {
    /// <summary>
    /// 用户上下文信息
    /// </summary>
    public static class UserContext {
        /// <summary>
        /// 内部字典
        /// </summary>
        private static readonly Dictionary<string, object> InnerDictionary = new Dictionary<string, object>();
        private static readonly object SyncRoot = new object();

        private static BaseInfo _defaultGlobal;

        /// <summary>
        /// 全局参数
        /// </summary>
        public static BaseInfo Default {
            get {
                if (_defaultGlobal == null) {
                    _defaultGlobal = new BaseInfo();
                }
                return _defaultGlobal;
            }
        }
        #region Nested type: BaseInfo

        /// <summary>
        /// 全局常量
        /// </summary>
        public class BaseInfo : IEditableObject {

            /// <summary>
            /// 系统日志路径
            /// </summary>
            [DisplayName("应用系统日志路径")]
            public string AppLogPath {
                get {
                    if (string.IsNullOrEmpty(UserAppDataPath)) {
                        return $"{ApplicationName}.EventLog_{DateTime.Today:yyyyMMdd}.log";
                    }
                    return Path.Combine(UserAppDataPath, $"{ApplicationName}.EventLog_{DateTime.Today:yyyyMMdd}.log");
                }
            }

            /// <summary>
            /// 应用程序 用户数据目录
            /// </summary>
            public string UserAppDataPath {
                get;
                set;
            }


            /// <summary>
            /// 仓库代码
            /// </summary>
            [DisplayName("仓库代码")]
            public string WhCode {
                get => GetProperty(string.Empty);
                set => SetProperty(value);
            }
            /// <summary>
            /// 仓库名称
            /// </summary>
            [DisplayName("仓库名称")]
            public string WhName {
                get => GetProperty(string.Empty);
                set => SetProperty(value);
            }

            /// <summary>
            /// 所在车间
            /// </summary>
            [DisplayName("所在车间")]
            public string Workshop {
                get => GetProperty(string.Empty);
                set => SetProperty(value);
            }

            /// <summary>
            /// 生产线
            /// </summary>
            [DisplayName("生产线")]
            public string ProductionLine {
                get => GetProperty(string.Empty);
                set => SetProperty(value);
            }

            /// <summary>
            /// 应用系统名称
            /// </summary>
            [DisplayName("应用系统名称")]
            public string ApplicationName {
                get => GetProperty(string.Empty);
                set => SetProperty(value);
            }

            /// <summary>
            /// 用户名
            /// </summary>
            [DisplayName("登录名")]
            public string UserName {
                get => GetProperty(Thread.CurrentPrincipal.Identity.Name);
                set => SetProperty(value);
            }

            /// <summary>
            /// 当前用户工号
            /// </summary>
            [DisplayName("工号")]
            public string EmployeeCode {
                get => GetProperty(Thread.CurrentPrincipal.Identity.Name);
                set => SetProperty(value);
            }

            /// <summary>
            /// 当前用户姓名全称
            /// </summary>
            [DisplayName("用户姓名")]
            public string UserFullName {
                get => GetProperty(Thread.CurrentPrincipal.Identity.Name);
                set => SetProperty(value);
            }

            /// <summary>
            /// 操作日期
            /// </summary>
            [DisplayName("操作日期")]
            public DateTime CurrentDate {
                get => GetProperty(DateTime.Today);
                set => SetProperty(value);
            }

            /// <summary>
            /// 远程服务基础Url
            /// </summary>
            [DisplayName("远程服务基础Url")]
            public string BaseUrl {
                get => GetProperty(string.Empty);
                set => SetProperty(value);
            }

            //public const string DefaultValueNoSetDepCode = "*未设置部门代码*";
            /// <summary>
            /// 用户所在部门代码
            /// </summary>
            [DisplayName("部门代码")]
            public string UserDepCode {
                get => GetProperty(string.Empty);
                set => SetProperty(value);
            }

            /// <summary>
            /// 用户所在部门名称
            /// </summary>
            [DisplayName("部门名称")]
            public string UserDepName {
                get => GetProperty(string.Empty);
                set => SetProperty(value);
            }

            /// <summary>
            /// 生产班组
            /// </summary>
            [DisplayName("生产班组")]
            public string WorkTeam {
                get => GetProperty(string.Empty);
                set => SetProperty(value);
            }

            public object this[string key] {
                get => InnerDictionary[key];

                set => SetProperty(value, key);
            }

            #region Methods

            private void SetProperty(object value, [CallerMemberName] string propertyName = null) {
                lock (SyncRoot) {
                    if (value == null) {
                        if (InnerDictionary.ContainsKey(propertyName)) {
                            InnerDictionary.Remove(propertyName);
                        }
                    } else if (InnerDictionary.ContainsKey(propertyName)) {
                        if (!InnerDictionary[propertyName].Equals(value)) {
                            InnerDictionary[propertyName] = value;
                            OnPropertyChanged(new PropertyChangingEventArgs(propertyName));
                        }
                    } else {
                        InnerDictionary.Add(propertyName, value);
                        OnPropertyChanged(new PropertyChangingEventArgs(propertyName));
                    }
                }
            }

            private T GetProperty<T>(T defaultValue, [CallerMemberName]string propertyName = null) {
                object value;
                if (InnerDictionary.TryGetValue(propertyName, out value)) {
                    return (T)value;
                }
                return defaultValue;
            }


            #endregion

            /// <summary>
            /// 属性值变更
            /// </summary>
            public event EventHandler<PropertyChangingEventArgs> PropertyChanged;

            private void OnPropertyChanged(PropertyChangingEventArgs e) {
                EventHandler<PropertyChangingEventArgs> handler = PropertyChanged;
                if (handler != null) {
                    handler(this, e);
                }
            }

            public event EventHandler ObjectChanged;

            private void OnObjectChanged(EventArgs e) {
                EventHandler handler = ObjectChanged;
                if (handler != null) {
                    handler(this, e);
                }
            }

            #region Implementation of IEditableObject

            public void BeginEdit() { }

            public void EndEdit() {
                OnObjectChanged(EventArgs.Empty);
            }

            public void CancelEdit() {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

    }
}