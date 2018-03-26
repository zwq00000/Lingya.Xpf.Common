using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using RedRiver.Data.Repos;

namespace Lingya.Xpf.Common {
    public abstract class ReadOnlyGridViewModelBase<TEntity> : DocumentViewModelBase where TEntity : class {
        static readonly Expression<Func<TEntity, bool>> AllAllowFilter = t => true;
        private Expression<Func<TEntity, bool>> _queryFilter = AllAllowFilter;
        private ICollection<TEntity> _entities;
        private TEntity _selectedEntity;

        protected ReadOnlyGridViewModelBase(IRepository<TEntity> repository) {
            Repository = repository;
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

        public IRepository<TEntity> Repository { get; }

        public virtual ICollection<TEntity> Entities {
            get { return _entities; }
            protected set {
                if (Equals(value, _entities)) return;
                _entities = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 当前选中的项
        /// </summary>
        public TEntity SelectedEntity {
            get { return _selectedEntity; }
            set {
                if (Equals(value, _selectedEntity)) return;
                _selectedEntity = value;
                OnPropertyChanged();
            }
        }

        #region  Commands

        [AsyncCommand(Name = "LoadCommand")]
        public virtual async void LoadDataAsync() {
            IsLoading = true;
            try {
                await LoadCoreAsync();
            } finally {
                IsLoading = false;
            }
        }
        
        [Command(Name = "RefreshCommand")]
        public void Refresh() {
            IsLoading = true;
            Entities = Repository.Query(QueryFilter).ToList();
            IsLoading = false;
        }

        #endregion

        protected virtual void OnEntitiesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }

        protected  override async Task LoadCoreAsync() {
            Entities = Repository.Query(QueryFilter).ToList();
            await Task.Yield();
        }
    }
}