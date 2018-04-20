using System;

namespace Lingya.Xpf.Core {
    internal class LoadingScope : IDisposable {
        private readonly ILoadingProvider _provider;
        /// <summary>
        /// �Ƿ��ɵ�ǰ�����޸ĵ� <see cref="ILoadingProvider.IsLoading"/> ״̬
        /// ԭ����˭�򿪵�˭����ر�
        /// </summary>
        private readonly bool _isHook = false;

        public LoadingScope(ILoadingProvider provider) {
            this._provider = provider;
            if (!provider.IsLoading) {
                provider.IsLoading = true;
                _isHook = true;
            }
        }

        #region IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            if (_isHook) {
                _provider.IsLoading = false;
            }
        }

        #endregion
    }
}