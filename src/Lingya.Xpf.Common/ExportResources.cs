using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lingya.Xpf {
    /// <summary>
    /// 导出的资源Uri
    /// </summary>
    public class ExportResources {
        private const string ResourcesFolder = "Resources";

        private static readonly string[] ExportResourcesNames = new[]
            {"CollectionViewResources.xaml", "CustomStylesResources.xaml", "EntityViewResources.xaml"};

        /// <summary>
        /// 获取导出的 公共资源 Uri
        /// 
        /// pack://application:,,,/ReferencedAssembly;component/Subfolder/ResourceFile.xaml
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Uri> GetResourcesUris() {
            var baseUri = GetAssemblyPackPath(typeof(ExportResources).Assembly);
            return ExportResourcesNames.Select(r => new Uri($"{baseUri}component/{ResourcesFolder}/{r}",UriKind.Absolute));
        }

        
        private static string GetAssemblyPackPath(Assembly assembly) {
            return $"pack://application:,,,/{assembly.GetName().Name};";
        }
    }
}