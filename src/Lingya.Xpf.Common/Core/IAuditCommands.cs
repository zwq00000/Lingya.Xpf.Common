using System;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

namespace Lingya.Xpf.Core {

    /// <summary>
    /// 加载过程支持
    /// </summary>
    public interface ILoadingProvider {

        /// <summary>
        /// 是否正在加载数据
        /// </summary>
        bool IsLoading { get; set; }
    }

    public interface IAuditCommands<TModel> {

        /// <summary>
        /// 审核单据
        /// </summary>
        ICommand<TModel> Audit { get; }

        /// <summary>
        /// 取消审核
        /// </summary>
        ICommand<TModel> CancelAudit { get; }

        /// <summary>
        /// 审核全部
        /// </summary>
        ICommand AuditAll { get; }
    }

}