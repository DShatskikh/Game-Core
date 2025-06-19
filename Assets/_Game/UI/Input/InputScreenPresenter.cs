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
        private IAssetLoader _assetLoader;

        [Inject]
        public void Construct(InputScreenView view, IAssetLoader assetLoader)
        {
            _view = view;
            _assetLoader = assetLoader;
            
#if UNITY_WEBGL || UNITY_ANDROID
            if (DeviceTypeDetector.IsMobile())
            {
                AwaitCreateMobileInput().Forget();
            }
#endif
        }

#if UNITY_WEBGL || UNITY_ANDROID
        private async UniTask AwaitCreateMobileInput()
        {
            await _assetLoader.InstantiateAsync(AssetPathConstants.JOYSTICK_PATH, _view.transform);
            await _assetLoader.InstantiateAsync(AssetPathConstants.RUN_BUTTON_PATH, _view.transform);
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