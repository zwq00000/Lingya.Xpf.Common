using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.ViewModel;
using Lingya.Xpf.Annotations;

namespace Lingya.Xpf.Common {
    /// <summary>
    ///                 <para>The base class for POCO view models that operate the collection of documents.
    /// </para>
    ///             </summary>
    public abstract class DocumentsViewModel<TModule> : INotifyPropertyChanged, ISupportLogicalLayout where TModule : ModuleDescription<TModule> {
        private const string ViewLayoutName = "DocumentViewModel";
        private bool _isLoading;
        protected bool DocumentChanging { get; private set; }

        /// <summary>
        ///                 <para>Navigation list that represents a collection of module descriptions.
        /// </para>
        ///             </summary>
        /// <value> </value>
        public TModule[] Modules { get; private set; }

        /// <summary>
        ///                 <para>A currently selected navigation list entry. This property is writable. When this property is assigned a new value, it triggers the navigating to the corresponding document.
        /// Since DocumentsViewModel is a POCO view model, this property will raise INotifyPropertyChanged.PropertyEvent when modified so it can be used as a binding source in views.
        /// </para>
        ///             </summary>
        /// <value> </value>
        public virtual TModule SelectedModule { get; set; }

        /// <summary>
        ///                 <para>A navigation list entry that corresponds to the currently active document. If the active document does not have the corresponding entry in the navigation list, the property value is null. This property is read-only.
        /// Since DocumentsViewModel is a POCO view model, this property will raise INotifyPropertyChanged.PropertyEvent when modified so it can be used as a binding source in views.
        /// </para>
        ///             </summary>
        /// <value> </value>
        public virtual TModule ActiveModule { get; protected set; }

        /// <summary>
        ///                 <para>Contains a current state of the navigation pane.
        /// </para>
        ///             </summary>
        /// <value> </value>
        public virtual NavigationPaneVisibility NavigationPaneVisibility { get; set; }

        protected virtual IDocumentManagerService DocumentManagerService {
            get {
                return this.GetService<IDocumentManagerService>();
            }
        }

        protected ILayoutSerializationService LayoutSerializationService {
            get {
                return this.GetService<ILayoutSerializationService>("RootLayoutSerializationService");
            }
        }

        protected IDocumentManagerService WorkspaceDocumentManagerService {
            get {
                return this.GetService<IDocumentManagerService>("WorkspaceDocumentManagerService");
            }
        }

        /// <summary>
        ///     <para> </para>
        /// </summary>
        /// <value> </value>
        public virtual TModule DefaultModule {
            get {
                return Modules.First<TModule>();
            }
        }

        protected bool IsLoaded { get; private set; }

        public bool IsLoading {
            get { return _isLoading; }
            set {
                if (value == _isLoading) return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        bool ISupportLogicalLayout.CanSerialize {
            get {
                return true;
            }
        }

        IDocumentManagerService ISupportLogicalLayout.DocumentManagerService {
            get {
                return DocumentManagerService;
            }
        }


        protected DocumentsViewModel() {
            Modules = CreateModules().ToArray<TModule>();
            foreach (TModule module in Modules)
                Messenger.Default.Register<NavigateMessage<TModule>>(this, module, x => Show(x.Token));
            Messenger.Default.Register<DestroyOrphanedDocumentsMessage>(this, x => DestroyOrphanedDocuments());
        }

        private void DestroyOrphanedDocuments() {
            foreach (IDocument document in this.GetOrphanedDocuments().Except<IDocument>(this.GetImmediateChildren(null))) {
                document.DestroyOnClose = true;
                document.Close(true);
            }
        }

        /// <summary>
        ///                 <para>Saves changes in all opened documents.
        /// Since DocumentsViewModel is a POCO view model, an instance of this class will also expose the SaveAllCommand property that can be used as a binding source in views.
        /// </para>
        ///             </summary>
        public void SaveAll() {
            Messenger.Default.Send<SaveAllMessage>(new SaveAllMessage());
        }

        /// <summary>
        ///                 <para>Used to close all opened documents and allows you to save unsaved results and to cancel closing.
        /// Since DocumentsViewModel is a POCO view model, an instance of this class will also expose the OnClosingCommand property that can be used as a binding source in views.
        /// </para>
        ///             </summary>
        /// <param name="cancelEventArgs">
        /// An argument of the System.ComponentModel.CancelEventArgs type which is used to cancel closing if needed.
        /// 
        ///           </param>
        public virtual void OnClosing(CancelEventArgs cancelEventArgs) {
            SaveLogicalLayout();
            if (LayoutSerializationService != null)
                PersistentLayoutHelper.PersistentViewsLayout["DocumentViewModel"] = LayoutSerializationService.Serialize();
            Messenger.Default.Send<CloseAllMessage>(new CloseAllMessage(cancelEventArgs, vm => true));
            PersistentLayoutHelper.SaveLayout();
        }

        public void Show(TModule module) {
            ShowCore(module);
        }

        public virtual IDocument ShowCore(TModule module) {
            IsLoading = true;
            try {
                if (module == null || DocumentManagerService == null)
                    return null;
                IDocument documentByIdOrCreate =
                    DocumentManagerService.FindDocumentByIdOrCreate(module.DocumentType, x => CreateDocument(module));
                documentByIdOrCreate.Show();
                return documentByIdOrCreate;
            } finally {
                IsLoading = false;
            }
        }

        public void PinPeekCollectionView(TModule module) {
            if (WorkspaceDocumentManagerService == null)
                return;
            WorkspaceDocumentManagerService.FindDocumentByIdOrCreate(module.DocumentType, x => CreatePinnedPeekCollectionDocument(module)).Show();
        }

        public virtual void OnLoaded(TModule module) {
            IsLoaded = true;
            DocumentManagerService.ActiveDocumentChanged += new ActiveDocumentChangedEventHandler(OnActiveDocumentChanged);
            if (!RestoreLogicalLayout())
                Show(module);
            PersistentLayoutHelper.TryDeserializeLayout(LayoutSerializationService, "DocumentViewModel");
        }

        private void OnActiveDocumentChanged(object sender, ActiveDocumentChangedEventArgs e) {
            if (e.NewDocument == null) {
                ActiveModule = default(TModule);
            } else {
                DocumentChanging = true;
                ActiveModule = Modules.FirstOrDefault<TModule>(m => m.DocumentType == (string)e.NewDocument.Id);
                DocumentChanging = false;
            }
        }

        protected virtual void OnSelectedModuleChanged(TModule oldModule) {
            if (!IsLoaded || DocumentChanging)
                return;
            Show(SelectedModule);
        }

        protected virtual void OnActiveModuleChanged(TModule oldModule) {
            SelectedModule = ActiveModule;
        }

        private IDocument CreateDocument(TModule module) {
            IDocument document = DocumentManagerService.CreateDocument(module.DocumentType, null, this);
            document.Title = GetModuleTitle(module);
            document.DestroyOnClose = false;
            return document;
        }

        protected virtual string GetModuleTitle(TModule module) {
            return module.ModuleTitle;
        }

        private IDocument CreatePinnedPeekCollectionDocument(TModule module) {
            IDocument document = WorkspaceDocumentManagerService.CreateDocument("PeekCollectionView", module.CreatePeekCollectionViewModel());
            document.Title = module.ModuleTitle;
            return document;
        }


        protected abstract TModule[] CreateModules();

        /// <summary>
        ///     <para> </para>
        /// </summary>
        public virtual void SaveLogicalLayout() {
            PersistentLayoutHelper.PersistentLogicalLayout = this.SerializeDocumentManagerService();
        }

        /// <summary>
        ///     <para> </para>
        /// </summary>
        /// <returns> </returns>
        public virtual bool RestoreLogicalLayout() {
            if (string.IsNullOrEmpty(PersistentLayoutHelper.PersistentLogicalLayout))
                return false;
            this.RestoreDocumentManagerService(PersistentLayoutHelper.PersistentLogicalLayout);
            return true;
        }

        #region Implement INotofyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName = null) {
            OnPropertyChanged(propertyName);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}