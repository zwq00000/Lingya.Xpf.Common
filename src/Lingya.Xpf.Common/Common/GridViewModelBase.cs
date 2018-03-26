using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using RedRiver.Data.Repos;

namespace Lingya.Xpf.Common {
    public abstract class GridViewModelBase<TEntity> : DocumentViewModelBase, IDisposable where TEntity : class {

        static readonly Expression<Func<TEntity, bool>> AllAllowFilter = t => true;
        private Expression<Func<TEntity, bool>> _queryFilter = AllAllowFilter;

        protected GridViewModelBase(IRepositoryLocal<TEntity> repository) {
            Repository = repository;
            Entities = Repository.Local;
            var notify = Repository as INotifyPropertyChanged;
            if (notify != null) {
                notify.PropertyChanged += (s, e) => {
                    DetactChanges();
                };
            }
        }

        protected Expression<Func<TEntity, bool>> QueryFilter {
            get { return _queryFilter; }
            set {
                if (value == null) {
                    value = AllAllowFilter;
                }
                _queryFilter = value;

            }
        }

        protected IRepositoryLocal<TEntity> Repository { get; }

        public virtual ICollection<TEntity> Entities {
            get;
        }

        /// <summary>
        /// 当前选中的项
        /// </summary>
        public TEntity SelectedEntity { get; set; }

        #region  Commands

        [AsyncCommand(Name = "LoadCommand")]
        public virtual async void Load() {
            await LoadCoreAsync();
        }

        [Command(Name = "SaveCommand")]
        public async Task Save() {
            IsLoading = true;
            try {
                await Task.Run(() => {
                    Repository.SaveChanges();
                    DetactChanges();
                });
            } finally {
                IsLoading = false;
            }
        }

        [Command(Name = "NewCommand")]
        public void AddNew() {
            var instance = Repository.Create();
            Repository.Local.Add(instance);
        }

        [Command(Name = "RefreshCommand")]
        public async Task Refresh() {
            await LoadCoreAsync();
        }

        /// <summary>
        /// 删除当前记录
        /// </summary>
        [Command(Name = "DeleteCommand")]
        public void Delete(int row) {
            if (SelectedEntity != null) {
                var result = ShowQuestion("是否删除当前记录?");
                if (result == MessageResult.Yes) {
                    Repository.Local.Remove(SelectedEntity);
                    Repository.SaveChanges();
                    DetactChanges();
                }
            }
        }

        /// <summary>
        /// 重置当前记录
        /// </summary>
        [Command(Name = "ResetCommand")]
        public void Reset() {
            Repository.Reload(SelectedEntity);
            DetactChanges();
        }

        public override void OnClose(CancelEventArgs e) {
            CheckNotSavedChanges(e);
        }

        public override void OnDestroy() {
            base.OnDestroy();
            Dispose();
        }

        #endregion

        /// <summary>
        /// 显示问题提示对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        protected MessageResult ShowQuestion(string message, MessageButton button = MessageButton.YesNo) {
            return MessageBoxService.ShowMessage(message, "提示信息", button,
                MessageIcon.Question);
        }

        protected virtual void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        protected override async Task LoadCoreAsync() {
            CancelEventArgs args = new CancelEventArgs();
            CheckNotSavedChanges(args);
            if (args.Cancel) {
                return;
            }
            try {
                IsLoading = true;
                await Repository.LoadAsync(QueryFilter);
                DetactChanges();
            } finally {
                IsLoading = false;
            }
        }

        private void CheckNotSavedChanges(CancelEventArgs args) {
            if (Repository.HasChanges()) {
                var result = ShowQuestion("当前更改没有保存,是否保存 ?", MessageButton.YesNoCancel);
                switch (result) {
                    case MessageResult.Yes:
                        Repository.SaveChanges();
                        break;
                    case MessageResult.No:
                        break;
                    case MessageResult.Cancel:
                        args.Cancel = true;
                        break;
                }
            }
        }

        protected virtual void OnSelectedEntityChanged(TEntity oldValue) {
            SelectedEntityChanged?.Invoke(this, new ValueChangedEventArgs<TEntity>(oldValue, SelectedEntity));
        }

        private void DetactChanges() {
            HasChanges = Repository.HasChanges();
        }

        public event EventHandler<ValueChangedEventArgs<TEntity>> SelectedEntityChanged;

        /*
        public virtual string CheckedItemType { get; set; }

        protected GridModuleNavigationParameter NavigationParameter { get; private set; }

        protected virtual void OnNavigatedFrom() { }

        protected virtual void OnNavigatedTo() {
            if (NavigationParameter == GridModuleNavigationParameter.NewItem)
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(ShowNewItemWindow));
        }

        public abstract void ShowNewItemWindow();

        #region ISupportNavigation

        void ISupportNavigation.OnNavigatedFrom() { OnNavigatedFrom(); }
        void ISupportNavigation.OnNavigatedTo() { OnNavigatedTo(); }

        object ISupportParameter.Parameter {
            get { return NavigationParameter; }
            set { NavigationParameter = value == null ? GridModuleNavigationParameter.Default : (GridModuleNavigationParameter)value; }
        }
        #endregion
        */

        #region IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual void Dispose() {
            var disposable = (IDisposable)Repository;
            disposable?.Dispose();
        }

        #endregion
    }

    public enum GridModuleNavigationParameter { Default, NewItem }
}