using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    // Управляет логикой открытия и закрытия окон
    public sealed class ScreenManager
    {
        private readonly ScreenConfig _config;
        private readonly Transform _parent;
        private readonly Dictionary<ScreensEnum, IScreenPresenter> _pair = new();
        private readonly DiContainer _container;

        // Внедрение зависимостей
        public ScreenManager(ScreenConfig config, Transform parent, DiContainer container)
        {
            _config = config;
            _parent = parent;
            _container = container;
        }

        // Открытие окна
        public IScreenPresenter Open(ScreensEnum screensEnum, DiContainer diContainer = null)
        {
            if (_pair.ContainsKey(screensEnum))
                return null;

            if (_config.TryGet(screensEnum, out ScreenData data))
            {
                var container = diContainer ?? _container;
                
                var screen = Object.Instantiate(data.Screen, _parent);
                var presenter = data.Presenter.Prototype();
                container.Inject(presenter, new []{screen});

                foreach (var monoBehaviour in screen.GetComponentsInChildren<MonoBehaviour>())
                {
                    container.Inject(monoBehaviour);  
                }
                
                _pair.Add(screensEnum, presenter);
                return presenter;
            }
            
            return null;
        }

        // Закрытие окна
        public void Close(ScreensEnum screensEnum)
        {
            if (!_pair.ContainsKey(screensEnum))
                return;
            
            _pair[screensEnum].Destroy();
            _pair.Remove(screensEnum);
        }
    }
}

