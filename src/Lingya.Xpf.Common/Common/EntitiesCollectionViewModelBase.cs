using System.Collections.Generic;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;

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
            await LoadDataCore();
        }
    }
}