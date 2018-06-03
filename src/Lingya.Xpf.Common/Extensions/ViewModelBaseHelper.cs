using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;
using Lingya.Xpf.Core;
using RedRiver.Data.Repos;

namespace Lingya.Xpf.Extensions {

    internal static class EntityResolveHelper {

        public static IEnumerable<T> QueryData<T>(this ILifetimeScope scope, Expression<Func<T, bool>> filter) {
            using (var child = scope.BeginLifetimeScope()) {
                var repo = child.Resolve<IRepository<T>>();
                repo.IsTracking = false;
                return repo.Query(filter).ToArray();
            }
        }
    }

    public static class ViewModelBaseHelper {

        public static IDisposable BeginLoadingScope(this ILoadingProvider provider) {
            return new LoadingScope(provider);
        }


        /// <summary>
        /// 显示问题提示对话框
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public static MessageResult ShowQuestion(this IDocumentContent owner, string message, MessageButton button = MessageButton.YesNo) {
            return ShowMessageInternal(owner, message, button, MessageIcon.Question);
        }

        public static MessageResult ShowMessage(this IDocumentContent owner, string message, MessageButton button = MessageButton.OK) {
            return owner.ShowMessageInternal(message, button, MessageIcon.Information);
        }

        public static MessageResult ShowWarnning(this IDocumentContent owner, string message, MessageButton button = MessageButton.OK) {
            return owner.ShowMessageInternal(message, button, MessageIcon.Warning);
        }

        public static MessageResult ShowMessageInternal(this IDocumentContent owner, string message, MessageButton button = MessageButton.OK, MessageIcon icon = MessageIcon.Information) {
            var service = owner.GetService<IMessageBoxService>();
            if (service != null) {
                return service.ShowMessage(message, "提示信息", button, MessageIcon.Information);
            } else {
                return MessageBox.Show(message, "提示信息", (MessageBoxButton)button, icon.ToMessageBoxImage()).ToMessageResult();
            }
        }

        private static MessageBoxImage ToMessageBoxImage(this MessageIcon icon) {
            switch (icon) {
                case MessageIcon.Question:
                    return MessageBoxImage.Question;
                //case MessageIcon.Asterisk:
                case MessageIcon.Information:
                    return MessageBoxImage.Information;

                case MessageIcon.Exclamation:
                    return MessageBoxImage.Exclamation;
                case MessageIcon.None:
                    return MessageBoxImage.None;
                //case MessageIcon.Hand:
                //case MessageIcon.Error:
                case MessageIcon.Stop:
                    return MessageBoxImage.Stop;

                default:
                    return MessageBoxImage.Information;
            }
        }

        private static MessageResult ToMessageResult(this MessageBoxResult result) {
            switch (result) {
                case MessageBoxResult.None:
                    return MessageResult.None;
                case MessageBoxResult.OK:
                    return MessageResult.OK;
                case MessageBoxResult.Cancel:
                    return MessageResult.Cancel;
                case MessageBoxResult.Yes:
                    return MessageResult.Yes;
                case MessageBoxResult.No:
                    return MessageResult.No;
                default:
                    return MessageResult.None;
            }
        }

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="owner"></param>
        /// <param name="preShowAction"></param>
        /// <param name="parameter"></param>
        public static void ShowDocument<TView>(this IDocumentContent owner, Action<IDocument> preShowAction = null, object parameter = null)
            where TView : UserControl {
            var doc = owner.FindDocumentOrCreate<TView>(parameter);
            preShowAction?.Invoke(doc);
            doc.Show();
        }

        /// <summary>
        /// 查找或创建窗体
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="owner"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static IDocument FindDocumentOrCreate<TView>(this IDocumentContent owner, object parameter = null)
            where TView : UserControl {
            var documentType = typeof(TView).Name;
            // 视图标识 docId 等于 documentType
            var docId = documentType;
            var service = owner.GetService<IDocumentManagerService>();
            return service.FindDocumentByIdOrCreate(docId, s => s.CreateDocument(documentType, parameter, owner));
        }

        public static void ShowDialog<TView>(this IDocumentContent owner,string title) where TView:UserControl {
            var documentType = typeof(TView).Name;
            owner.GetService<IDialogService>().ShowDialog(new UICommand[0], title, documentType, null, null, owner);
        }

        public static void ShowDialog<TView>(this IDocumentContent owner, string title,object parameter) where TView : UserControl {
            var documentType = typeof(TView).Name;
            owner.GetService<IDialogService>().ShowDialog(new UICommand[0], title, documentType, null,parameter, owner);
        }

        public static void ShowDialog<TView>(this IDocumentContent owner, string title,object viewModel, object parameter) where TView : UserControl {
            var documentType = typeof(TView).Name;
            owner.GetService<IDialogService>().ShowDialog(new UICommand[0], title, documentType,viewModel, parameter, owner);
        }

        private static IDialogService GetDialogService(this IDocumentContent owner) {
            var service = owner.GetRequiredService<IDialogService>();
            if (service == null) {
                service = new DialogService();
            }

            return service;
        }
    }
}