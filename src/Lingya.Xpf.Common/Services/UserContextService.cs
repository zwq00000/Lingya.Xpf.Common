using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Lingya.Xpf.Services {
    public class UserContextService:INotifyPropertyChanged{
        /// <summary>
        /// 内部字典
        /// </summary>
        private static readonly Dictionary<string, object> InnerDictionary = new Dictionary<string, object>();
        private static readonly object SyncRoot = new object();

        protected UserContextService() {

        }

        #region Methods

        protected void SetProperty(object value, [CallerMemberName] string propertyName = null) {
            lock (SyncRoot) {
                if (value == null) {
                    if (InnerDictionary.ContainsKey(propertyName)) {
                        InnerDictionary.Remove(propertyName);
                    }
                } else if (InnerDictionary.ContainsKey(propertyName)) {
                    if (!InnerDictionary[propertyName].Equals(value)) {
                        InnerDictionary[propertyName] = value;
                        OnPropertyChanged(propertyName);
                    }
                } else {
                    InnerDictionary.Add(propertyName, value);
                    OnPropertyChanged(propertyName);
                }
            }
        }

        protected T GetProperty<T>(T defaultValue, [CallerMemberName]string propertyName = null) {
            if (InnerDictionary.TryGetValue(propertyName, out var value)) {
                return (T)value;
            }
            return defaultValue;
        }


        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}