using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    // Логика основного окна игры
    [Serializable]
    public sealed class InputScreenPresenter : IScreenPresenter, IGameCutsceneListener, IGameBattleListener
    {
        private InputScreenView _view;
        private AssetLoader _assetLoader;

        [Inject]
        public void Construct(InputScreenView view, AssetLoader assetLoader)
        {
            _view = view;
            _assetLoader = assetLoader;
            
#if UNITY_WEBGL || UNITY_ANDROID
            if (DeviceTypeDetector.IsMobile())
            {
                AwaitCreateJoystick().Forget();
            }
#endif
        }

#if UNITY_WEBGL || UNITY_ANDROID
        private async UniTask AwaitCreateJoystick()
        {
            await _assetLoader.InstantiateAsync(AssetPathConstants.JOYSTICK_PATH, _view.transform);
        }
#endif
        
        public IScreenPresenter Prototype() => 
            new InputScreenPresenter();

        public void Destroy()
        {
            Object.Destroy(_view.gameObject);
        }
        
        void IGameCutsceneListener.OnShowCutscene() => 
            _view.ToggleActivate(false);
        
        void IGameCutsceneListener.OnHideCutscene() => 
            _view.ToggleActivate(true);
        
        void IGameBattleListener.OnOpenBattle() => 
            _view.ToggleActivate(false);
        
        void IGameBattleListener.OnCloseBattle() => 
            _view.ToggleActivate(true);
    }
}