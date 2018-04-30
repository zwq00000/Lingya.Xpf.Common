using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using JetBrains.Annotations;
using Lingya.Xpf.Extensions;
using RedRiver.Data.Repos;

namespace Lingya.Xpf.Common {
    public abstract class GridViewModelBase<TEntity> : DocumentViewModelBase where TEntity : class {

        static readonly Expression<Func<TEntity, bool>> AllAllowFilter = t => true;
        private Expression<Func<TEntity, bool>> _queryFilter = AllAllowFilter;
        private TEntity _selectedEntity;

        protected GridViewModelBase() {
            if (!ViewModelBase.IsInDesignMode) {
                Repository = Scope.Resolve<IRepositoryLocal<TEntity>>();
                Entities = Repository.Local;
                if (Repository is INotifyPropertyChanged propertyChanged) {
                    propertyChanged.PropertyChanged += (s, e) => { DetactChanges(); };
                }
            }
        }


        protected GridViewModelBase([NotNull] ILifetimeScope scope, [NotNull]IRepositoryLocal<TEntity> repository):base(scope) {
            Repository = repository;
            Entities = Repository.Local;
            if (Repository is INotifyPropertyChanged propertyChanged) {
                propertyChanged.PropertyChanged += (s, e) => {
                    DetactChanges();
                };
            }
        }

        public Expression<Func<TEntity, bool>> QueryFilter {
            get { return _queryFilter; }
            set {
                if (value == null) {
                    value = AllAllowFilter;
                }
                _queryFilter = value;

            }
        }

        protected IRepositoryLocal<TEntity> Repository { get; }

        /// <summary>
        /// 数据实体集合
        /// </summary>
        public virtual ICollection<TEntity> Entities {
            get;
        }

        /// <summary>
        /// 当前选中的项
        /// </summary>
        public TEntity SelectedEntity {
            get => _selectedEntity;
            set {
                if (Equals(value, _selectedEntity)) return;
                _selectedEntity = value;
                OnPropertyChanged();
            }
        }

        #region  Commands

        [AsyncCommand(Name = "LoadCommand")]
        public virtual async void LoadData() {
            using (this.BeginLoadingScope()) {
                await LoadDataCore();
            }
        }

        [Command(Name = "SaveCommand")]
        public async Task Save() {
            using (this.BeginLoadingScope()) {
#if NET40
                await Task.Factory.StartNew(() => {
                    Repository.SaveChanges();
                    DetactChanges();
                });
#else
                await Task.Run(() => {
                    Repository.SaveChanges();
                    DetactChanges();
                });
#endif

            }
        }

        /// <summary>
        /// 新建对象
        /// </summary>
        [Command(Name = "NewCommand")]
        public virtual void New() {
            var instance = Repository.Create();
            Repository.Local.Add(instance);
        }

        /// <summary>
        /// 刷新对象
        /// </summary>
        /// <returns></returns>
        [Command(Name = "RefreshCommand")]
        public virtual async Task Refresh() {
            using (this.BeginLoadingScope()) {
                await LoadDataCore();
            }
        }

        /// <summary>
        /// 删除当前记录
        /// </summary>
        [Command(Name = "DeleteCommand")]
        public void Delete(TEntity entity) {
            if (entity != null) {
                var result = this.ShowQuestion("是否删除当前记录?");
                if (result == MessageResult.Yes) {
                    using (this.BeginLoadingScope()) {
                        try {
                            Entities.Remove(entity);
                            Repository.SaveChanges();
                            DetactChanges();
                        } catch (InvalidOperationException) {
                            Repository.SaveChanges();
                            DetactChanges();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否允许删除对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool CanDelete(TEntity entity) {
            return entity!=null;
        }

        /// <summary>
        /// 重置当前记录
        /// </summary>
        [Command(Name = "ResetCommand")]
        public async Task Reset(TEntity entity) {
            if (entity != null) {
                await Repository.ReloadAsync(entity);
                DetactChanges();
            }
        }

        public override void OnClose(CancelEventArgs e) {
            CheckNotSavedChanges(e);
        }

        #endregion



        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        protected override async Task LoadDataCore() {
            CancelEventArgs args = new CancelEventArgs();
            CheckNotSavedChanges(args);
            if (args.Cancel) {
                return;
            }
            await Repository.LoadAsync(QueryFilter);
            DetactChanges();
        }

        private void CheckNotSavedChanges(CancelEventArgs args) {
            if (Repository.HasChanges()) {
                var result = this.ShowQuestion("当前更改没有保存,是否保存 ?", MessageButton.YesNoCancel);
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

        private void DetactChanges() {
            HasChanges = Repository.HasChanges();
        }
    }

    public enum GridModuleNavigationParameter { Default, NewItem }
}