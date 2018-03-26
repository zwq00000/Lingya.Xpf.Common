using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using RedRiver.Data.Repos;

namespace Lingya.Xpf.Common {

    public abstract class EntitiesCollectionViewModelBase<TEntity>: DocumentViewModelBase where TEntity : class {
        
        private ICollection<TEntity> _entities;

        public virtual ICollection<TEntity> Entities {
            get { return _entities; }
            protected set {
                if (Equals(value, _entities)) return;
                _entities = value;
                OnPropertyChanged();
            }
        }


        [AsyncCommand(Name = "RefreshCommand")]
        public async Task Refresh() {
            await LoadCoreAsync();
        }
    }

    public abstract class EntityViewModelBase<TEntity>: ViewModelBase, IDocumentContent where TEntity : class{


        protected EntityViewModelBase(IRepository<TEntity> repository, TEntity entity) {
            Repository = repository;
            Entity = entity;

        }

        /// <summary>
        ///                 <para>An entity represented by this view model.
        /// </para>
        ///             </summary>
        /// <value> </value>
        public virtual TEntity Entity { get; protected set; }

        private string ViewName {
            get {
                return typeof(TEntity).Name + "View";
            }
        }

        public IRepository<TEntity> Repository { get; }

        public virtual IQueryable<TEntity> Source { get; protected set; }

        #region Implementation of IDocumentContent

        /// <inheritdoc />
        public void OnClose(CancelEventArgs e) {
            
        }

        /// <inheritdoc />
        public void OnDestroy() {
            
        }

        /// <inheritdoc />
        public IDocumentOwner DocumentOwner { get; set; }

        /// <inheritdoc />
        public object Title { get; }

        #endregion
    }
}