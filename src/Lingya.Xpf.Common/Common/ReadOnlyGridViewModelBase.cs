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
    /// ֻ�������ݱ����ͼģ�ͻ������ͣ������ṩ������<see cref="IRepositoryAsync{TEntity}"/>
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
        /// ��ѯ������
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
        /// �洢��ʵ��
        /// </summary>
        public IRepositoryAsync<TEntity> Repository { get; }

        /// <summary>
        /// ʵ����󼯺�
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
        /// ��ǰѡ�е���
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
        /// �������ݣ��ڲ����� <see cref="LoadDataCore"/>
        /// </summary>
        /// <returns></returns>
        [AsyncCommand(Name = "LoadCommand")]
        public virtual async Task LoadData() {
            using (this.BeginLoadingScope()) {
                await LoadDataCore();
            }
        }
        
        /// <summary>
        /// ˢ������ ��ͬ��<see cref="LoadData"/>
        /// </summary>
        /// <returns></returns>
        [AsyncCommand(Name = "RefreshCommand")]
        public virtual async Task Refresh() {
            await LoadData();
        }

        #endregion

        /// <summary>
        /// ���ݼ���,������ <see ref="Entities"/>
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