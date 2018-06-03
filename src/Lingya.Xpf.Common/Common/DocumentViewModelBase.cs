using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Autofac;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Utils.Design;
using JetBrains.Annotations;
using Lingya.Xpf.Core;
using Lingya.Xpf.Extensions;
using Lingya.Xpf.Services;

namespace Lingya.Xpf.Common {
    /// <summary>
    /// 文档内容 ViewmModel 基础
    /// </summary>
    [POCOViewModel]
    public abstract class DocumentViewModelBase :  IDocumentContent,ISupportParameter, INotifyPropertyChanged ,ILoadingProvider{
        private bool _isLoading;
        private bool _hasChanges;
        private object _title;
        private bool _isInitialized = false;

        protected DocumentViewModelBase() {
            if (!ViewModelBase.IsInDesignMode) {
                Scope = this.BeginLifetimeScope();
            }
        }

        protected DocumentViewModelBase(ILifetimeScope scope) {
              this.Scope = scope;
        }

        #region Service
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

       

        #endregion

        #region Properties

        public bool IsLoading {
            get { return _isLoading; }
            set {
                if (value == _isLoading) return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 数据是否改变
        /// </summary>
        public virtual bool HasChanges {
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
        /// 异步加载数据
        /// </summary>
        /// <returns></returns>
        protected abstract Task LoadDataCore();

        #region Commands

        /// <summary>
        /// 在 <see cref="OnInitialized">初始化</see>之前，处理视图模型参数，
        /// 此方法在加载数据 <see cref="LoadDataCore"/> 之前
        /// <see cref="ISupportParameter.Parameter"/>
        /// </summary>
        /// /// <remarks><see cref="Parameter"/> Can be Null</remarks>
        protected virtual void ProcessParameter() {
        }

        /// <summary>
        /// 当初始化完成时,响应此方法，
        /// 按顺序调用 <see cref="ProcessParameter"/>和<see cref="LoadDataCore"/> 方法
        /// </summary>
        /// <returns></returns>
        protected virtual async Task OnInitialized() {
            using (this.BeginLoadingScope()) {
                ProcessParameter();
                await LoadDataCore();
            }
        }

        [AsyncCommand]
        public virtual async  Task OnLoaded() {
            if (!_isInitialized) {
                //第一次加载执行初始化事件
                await OnInitialized();
                
                _isInitialized = true;
            }
        }

        [Command]
        public virtual void OnUnloaded() {
            Debug.WriteLine($"{GetType().Name}.OnUnloaded");
        }

        #endregion Commands


        #region Implements INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
#if NET40
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#else
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#endif


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
            Scope?.Dispose();
            GC.Collect();
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

        protected ILifetimeScope Scope { get; }

        #endregion

        #region Implementation of ISupportParameter

        /// <summary>
        ///                 <para>Specifies a parameter for passing data between view models.
        /// </para>
        ///             </summary>
        /// <value>A parameter to be passed.</value>
        public object Parameter { get; set; }

        #endregion
    }
}