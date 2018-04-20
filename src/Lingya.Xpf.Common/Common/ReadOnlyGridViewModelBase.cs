using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Lingya.Xpf.Extensions;
using Lingya.Xpf.Services;
using RedRiver.Data.Repos;

namespace Lingya.Xpf.Common {
    /// <summary>
    /// 只读的数据表格视图模型基础类型，数据提供者依赖<see cref="IRepositoryAsync{TEntity}"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ReadOnlyGridViewModelBase<TEntity> : DocumentViewModelBase where TEntity : class {
        static readonly Expression<Func<TEntity, bool>> AllAllowFilter = t => true;
        private Expression<Func<TEntity, bool>> _queryFilter = AllAllowFilter;
        private ICollection<TEntity> _entities;
        private TEntity _selectedEntity;

        protected ReadOnlyGridViewModelBase() {
            if (!ViewModelBase.IsInDesignMode) {
                this.Repository = this.ScopeAndResolve<IRepositoryAsync<TEntity>>();
                Repository.IsTracking = false;
            }
        }

        protected ReadOnlyGridViewModelBase(IRepositoryAsync<TEntity> repository) {
            Repository = repository;
            Repository.IsTracking = false;
        }

        /// <summary>
        /// 查询过滤器
        /// </summary>
        public Expression<Func<TEntity, bool>> QueryFilter {
            get { return _queryFilter; }
            set {
                if (value == null) {
                    value = AllAllowFilter;
                }
                _queryFilter = value;
            }
        }

        /// <summary>
        /// 存储库实例
        /// </summary>
        public IRepositoryAsync<TEntity> Repository { get; }

        /// <summary>
        /// 实体对象集合
        /// </summary>
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

        /// <summary>
        /// 加载数据，内部调用 <see cref="LoadDataCore"/>
        /// </summary>
        /// <returns></returns>
        [AsyncCommand(Name = "LoadCommand")]
        public virtual async Task LoadData() {
            using (this.BeginLoadingScope()) {
                await LoadDataCore();
            }
        }
        
        /// <summary>
        /// 刷新数据 等同于<see cref="LoadData"/>
        /// </summary>
        /// <returns></returns>
        [AsyncCommand(Name = "RefreshCommand")]
        public virtual async Task Refresh() {
            await LoadData();
        }

        #endregion

        /// <summary>
        /// 数据加载,负责构造 <see ref="Entities"/>
        /// </summary>
        /// <returns></returns>
        protected  override async Task LoadDataCore() {
            Entities = await Repository.ToListAsync(this.QueryFilter);
        }

        /// <summary>
        /// <para>Invoked after a document has been closed (hidden). </para>
        /// </summary>
        public override void OnDestroy() {
            base.OnDestroy();
        }
    }
}