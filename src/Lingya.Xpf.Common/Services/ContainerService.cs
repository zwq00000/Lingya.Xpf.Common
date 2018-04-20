using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using IContainer = Autofac.IContainer;

namespace Lingya.Xpf.Services {
    public static class ContainerService{
        private static IContainer _container;


        public static void RegisteContainer(IContainer container) {
            _container = container;
        }

        public static ILifetimeScope BeginLifetimeScope<TModel>(this TModel tag) {
            return _container.BeginLifetimeScope(typeof(TModel).Name);
        }

        public static T ScopeAndResolve<T>(this object tag) {
            var scope = BeginLifetimeScope(tag);
            if (tag is IDisposable disposable) {
                scope.Disposer.AddInstanceForDisposal(disposable);
            }
             
            return scope.Resolve<T>();
        }

    }
}
