using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lingya.Xpf.Extensions {
    internal static class ImageUrlHelper {

        public static Uri GetImageUri(string imageFile) {
            return new System.Uri($"{typeof(ImageUrlHelper).Assembly.GetAssemblyPackPath()};component/Images/{imageFile}", System.UriKind.RelativeOrAbsolute);
        }

        private static string GetAssemblyPackPath(this Assembly assembly) {
            return $"pack://application:,,,/{assembly.GetName().Name};";
        }
    }
}
