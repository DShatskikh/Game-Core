using UnityEngine;
using Zenject;

namespace Game
{
    public class OpenShop : MonoBehaviour
    {
        [SerializeField]
        private ShopView _shopView;

        [SerializeField]
        private ShopButton[] _buttons;

        [SerializeField]
        private ShopButton _prefab;

        [SerializeField]
        private Product[] _products;
        
        private DiContainer _diContainer;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        private void Start()
        {
            _diContainer.BindFactory<ShopPresenter, ShopPresenter.ShopPresenterFactory>()
                .WithArguments(_shopView, _buttons, _prefab, _products);

            var factory = _diContainer.TryResolve<ShopPresenter.ShopPresenterFactory>();
            factory.Create();
        }
    }
}