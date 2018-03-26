using System.Collections.Generic;

namespace Lingya.Xpf.Common {
    public class ModuleGroup {
        public ModuleGroup(string title, IEnumerable<ModuleInfo> moduleInfos) {
            Title = title;
            ModuleInfos = moduleInfos;
        }
        /// <summary>
        /// ����
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// ģ���б�
        /// </summary>
        public IEnumerable<ModuleInfo> ModuleInfos { get; private set; }
    }
}