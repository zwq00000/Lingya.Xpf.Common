using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;

namespace Lingya.Xpf.Common {

    public class ModuleInfoRegister {

        private static readonly IDictionary<string,ModuleInfo> ModuleInfos = new ConcurrentDictionary<string, ModuleInfo>();

        public static bool Register(ModuleInfo module) {
            if (module == null) {
                throw new ArgumentNullException(nameof(module));
            }

            if (!ModuleInfos.ContainsKey(module.DocumentType)) {
                ModuleInfos.Add(module.DocumentType, module);
                return true;
            }

            return false;
        }

        public static ModuleInfo FindModuleInfo(string documentType) {
            if (ModuleInfos.TryGetValue(documentType, out var module)) {
                return module;
            }
            return null;
        }

        public static ModuleInfo FindModuleInfo(Type viewType) {
            var documentType = viewType.Name;
            if (ModuleInfos.TryGetValue(documentType, out var module)) {
                return module;
            }
            return null;
        }

        public static ModuleInfo FindModuleInfo(object key) {
            if (key is ModuleInfo info) {
                return info;
            }
            if (key is string s) {
                return FindModuleInfo(s);
            }

            if (key is Type type) {
                return FindModuleInfo(type);
            }

            return null;
        }
    }

    public class ModuleInfo {
        /// <summary>
        /// 视图类型
        /// </summary>
        private Type _viewType;

        public ModuleInfo() {
        }

        public ModuleInfo(string documentType, string title) : this() {
            DocumentType = documentType;
            Title = title;
            ModuleInfoRegister.Register(this);
        }

        public ModuleInfo(Type viewType, string title) : this() {
            ViewType = viewType;
            Title = title;
            ModuleInfoRegister.Register(this);
        }

        /// <summary>
        /// View 类型名称
        /// </summary>
        public string DocumentType { get;  set; }

        /// <summary>
        /// 视图类型
        /// </summary>
        /// <summary>获取或设置此 <see cref="T:System.Windows.DataTemplate" /> 所针对的类型。</summary>
        /// <returns>默认值为 null。</returns>
        [DefaultValue(null)]
        [Ambient]
        public Type ViewType {
            get => _viewType;
            set {
                _viewType = value;
                if (value != null) {
                    DocumentType = value.Name;
                }
            }
        }

        public Type ViewModelType { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }


        public ImageSource Icon { get; set; }

    }
}