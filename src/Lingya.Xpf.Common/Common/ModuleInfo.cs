using System;
using System.Windows.Controls;
using DevExpress.Mvvm;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Native;

namespace Lingya.Xpf.Common {

    public class ModuleInfo<TView> : ModuleInfo where TView : Control {

        public ModuleInfo(ISupportServices parent, string title) : base(typeof(TView).Name, parent, title) {

        }

        #region Overrides of ModuleInfo

        /// <inheritdoc />
        public override void Show(object parameter = null) {
            var service = Parent.ServiceContainer.GetRequiredService<IDocumentManagerService>();
            if (service == null) {
                throw new NullReferenceException("not found IDocumentManagerService");
            }
            var id = parameter == null ? DocumentType : $"{DocumentType}_{parameter}";
            var doc = service.FindDocumentByIdOrCreate(id, s => s.CreateDocument(DocumentType, parameter, Parent));
            doc.DestroyOnClose = true;
            SetCaptionImage(doc);
            var docContent = doc.Content as DocumentViewModelBase;
            if (docContent != null) {
                docContent.Title = Title;
            }
            doc.Show();
        }

        #endregion
    }

    public abstract class ModuleInfo {
        protected readonly ISupportServices Parent;
        protected static readonly Uri DefaultIconUri = DXImageHelper.GetImageUri("New", ImageSize.Size16x16);

        private ModuleInfo() {
            ShowCommand = new DelegateCommand<object>(Show);
        }

        public ModuleInfo(string documentType, ISupportServices parent, string title) : this() {
            DocumentType = documentType;
            Parent = parent;
            Title = title;
        }

        /// <summary>
        /// View 类型名称
        /// </summary>
        public string DocumentType { get; private set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Model 图标 Uri
        /// </summary>
        public virtual Uri Icon { get; set; } = DefaultIconUri;

        internal ModuleInfo SetIcon(string icon) {
            Icon = AssemblyHelper.GetResourceUri(typeof(ModuleInfo).Assembly, $"Images/{icon}.png");
            return this;
        }

        /// <summary>
        /// 显示/打开 模块
        /// </summary>
        /// <param name="parameter"></param>
        public abstract void Show(object parameter = null);

        public ICommand<object> ShowCommand { get; }

        protected void SetCaptionImage(IDocument doc) {
            var document = doc as DockingDocumentUIServiceBase<DocumentPanel, DocumentGroup>.Document;
            if (document != null) {
                if (Icon == null) {
                    Icon = DefaultIconUri;
                }
                var image = ImageSourceHelper.GetImageSource(Icon);
                document.DocumentPanel.CaptionImage = image;
                document.DocumentPanel.ShowCaptionImage = true;
            }
        }
    }
}