using UnityEngine;
using Zenject;

namespace Game
{
    public class EnderChestToggleHandler : IGameEnderChestListener
    {
        private readonly Transform _container;
        private readonly EnderChestScreen _prefab;
        private readonly DiContainer _diContainer;

        private EnderChestScreen _screen;

        public EnderChestToggleHandler(DiContainer diContainer, Transform container, EnderChestScreen prefab)
        {
            _diContainer = diContainer;
            _container = container;
            _prefab = prefab;
        }

        public void OnOpenEnderChest()
        {
            _screen = Object.Instantiate(_prefab, _container);
            _diContainer.Inject(_screen);
        }

        public void OnCloseEnderChest()
        {
            if (_screen)
                Object.Destroy(_screen.gameObject);
        }
    }
}