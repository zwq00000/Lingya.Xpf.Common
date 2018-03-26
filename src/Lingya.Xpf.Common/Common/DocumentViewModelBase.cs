using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using Lingya.Xpf.Annotations;

namespace Lingya.Xpf.Common {
    /// <summary>
    /// 文档内容 ViewmModel 基础
    /// </summary>
    [POCOViewModel]
    public abstract class DocumentViewModelBase : IDocumentContent, INotifyPropertyChanged {
        private bool _isLoading;
        private bool _hasChanges;
        private object _title;

        #region Properties

        protected IMessageBoxService MessageBoxService {
            get {
                return this.GetRequiredService<IMessageBoxService>();
            }
        }

        protected virtual IDocumentManagerService DocumentManagerService {
            get {
                return this.GetService<IDocumentManagerService>();
            }
        }

        protected IDocumentManagerService WindowedDocumentManagerService {
            get {
                return this.GetService<IDocumentManagerService>("WindowedDocumentUIService");
            }
        }

        public bool IsLoading {
            get { return _isLoading; }
            set {
                if (value == _isLoading) return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool HasChanges {
            get { return _hasChanges; }
            set {
                if (value == _hasChanges) return;
                _hasChanges = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Title));
            }
        }

        #endregion

        /// <summary>
        /// 初始化数据源
        /// </summary>
        /// <returns></returns>
        protected abstract Task LoadCoreAsync();

        [AsyncCommand]
        public async void OnInitialized() {
            IsLoading = true;
            try {
                await LoadCoreAsync();
            } finally {
                IsLoading = false;
            }
        }

        [Command]
        public virtual void OnLoaded() {
            Debug.WriteLine($"{GetType().Name}.Onloaded");
        }

        [Command]
        public virtual void OnUnloaded() {
            Debug.WriteLine($"{GetType().Name}.OnUnloaded");
        }

        #region Implements INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChanged(string propertyName) {
            OnPropertyChanged(propertyName);
        }

        #endregion

        #region Implementation of IDocumentContent

        /// <summary>
        ///  <para>Invoked before a document is closed (hidden), and allows you to prevent this action. </para>
        ///  </summary>
        /// <param name="e">
        /// Provides data for the event handler that can be used to prevent specific operations on a document.
        ///           </param>
        public virtual void OnClose(CancelEventArgs e) {

        }

        /// <summary>
        /// <para>Invoked after a document has been closed (hidden). </para>
        /// </summary>
        public virtual void OnDestroy() {
        }

        /// <summary>
        /// <para>Gets or sets the service which owns the current document.
        /// </para>
        /// </summary>
        /// <value>An <see cref="T:DevExpress.Mvvm.IDocumentOwner" /> implementation that represents the service to which the current document belongs.
        /// </value>
        public IDocumentOwner DocumentOwner { get; set; }

        /// <summary>
        /// <para>Gets a value specifying the document title. </para>
        /// </summary>
        /// <value><para>The text displayed in the document title.</para>
        /// </value>
        public object Title {
            get {
                if (_title == null) {
                    return HasChanges ? "*":string.Empty;
                }
                if (HasChanges) {
                    return _title + " *";
                }
                return _title;
            }
            set {
                if (Equals(value, _title)) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}