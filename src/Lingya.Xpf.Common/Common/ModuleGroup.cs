using System.Collections.Generic;

namespace Lingya.Xpf.Common {
    public class ModuleGroup {
        public ModuleGroup(string title, IEnumerable<ModuleInfo> moduleInfos) {
            Title = title;
            ModuleInfos = moduleInfos;
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// 模块列表
        /// </summary>
        public IEnumerable<ModuleInfo> ModuleInfos { get; private set; }
    }
}