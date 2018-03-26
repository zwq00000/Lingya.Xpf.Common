using System;
using System.Windows;
using DevExpress.Mvvm.UI;

namespace Lingya.Xpf.Common {
    public class ResourcesViewLocator :IViewLocator {

        public ResourceDictionary Resource { get; set; }

        Type IViewLocator.ResolveViewType(string name) {
            throw new NotSupportedException();
        }

        object IViewLocator.ResolveView(string name) {
            //return (object)this.GetValueOrDefault<string, DataTemplate>(name).With<DataTemplate, DependencyObject>((Func<DataTemplate, DependencyObject>)(x => x.LoadContent()));
            ResourceDictionary res = Resource;
            if (res == null) {
                res = Application.Current.Resources;
            }
            return FindResources(res, name);
        }

        private object FindResources(ResourceDictionary resource, string name) {
            foreach (DataTemplateKey key in resource.Keys) {
                var dataType = (Type) key.DataType;
                if (dataType == null) {
                    continue;
                }

                if (String.Equals(dataType.Name,name)) {
                    return ((DataTemplate) resource[key]).LoadContent();
                }
            }
            foreach (var mergedDictionary in resource.MergedDictionaries) {
                var result = FindResources(mergedDictionary, name);
                if (result != null) {
                    return result;
                }
            }
            return null;
        }

        string IViewLocator.GetViewTypeName(Type type) {
            return type.Name;
        }
    }
}