using System;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    // Логика основного окна игры
    [Serializable]
    public sealed class InputScreenPresenter : IScreenPresenter, IGameCutsceneListener, IGameBattleListener
    {
        private InputScreenView _view;

        [Inject]
        public void Construct(InputScreenView view)
        {
            _view = view;

            if (DeviceTypeDetector.IsMobile()) 
                _view.ShowJoystick();
        }
        
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