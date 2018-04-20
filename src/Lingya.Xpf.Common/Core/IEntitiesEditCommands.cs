using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;

namespace Lingya.Xpf.Core {
    /// <summary>
    /// 实体对象集合编辑命令接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntitiesEditCommands<in TEntity> {
        [Command(Name = "SaveCommand")]
        Task Save();

        /// <summary>
        /// 是否可以保存
        /// </summary>
        /// <returns></returns>
        bool CanSave();

        /// <summary>
        /// 新建对象
        /// </summary>
        [Command(Name = "NewCommand")]
        void New();

        /// <summary>
        /// 从数据库重新加载实体
        /// </summary>
        /// <returns></returns>
        [Command(Name = "RefreshCommand")]
        Task Refresh();

        /// <summary>
        /// 删除当前记录
        /// </summary>
        [Command(Name = "DeleteCommand")]
        void Delete(TEntity entity);

        /// <summary>
        /// 是否允许删除对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool CanDelete(TEntity entity);
    }
}