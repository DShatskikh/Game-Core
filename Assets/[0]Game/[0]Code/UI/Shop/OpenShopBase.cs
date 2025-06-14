using System.Collections;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game
{
    public abstract class OpenShopBase<TShop, TFactory> : MonoBehaviour where TFactory : PlaceholderFactory<TShop>
    {
        [SerializeField]
        private protected ShopView _shopViewPrefab;

        [SerializeField]
        private protected ShopButton _prefab;

        [SerializeField]
        private protected SerializableDictionary<string, LocalizedString> _localizedStrings;

        [SerializeField]
        private protected ShopBackground _background;

        [Inject]
        private protected DiContainer _diContainer;
        
        [Button]
        public virtual void Open()
        {
            gameObject.SetActive(true);
            AwaitOpen().Forget();
        }

        private protected async UniTask AwaitOpen()
        {
            await UniTask.DelayFrame(1);
            Binding();

            var factory = _diContainer.TryResolve<TFactory>();
            factory.Create();
            _diContainer.Unbind<TFactory>();
        }
        
        private protected abstract void Binding();
    }
}