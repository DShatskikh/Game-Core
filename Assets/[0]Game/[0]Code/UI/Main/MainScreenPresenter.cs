using System;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    [Serializable]
    public sealed class MainScreenPresenter : IScreenPresenter, IGameCutsceneListener, IGameBattleListener
    {
        private MainScreenView _view;

        [Inject]
        public void Construct(MainScreenView view)
        {
            _view = view;
        }
        
        public IScreenPresenter Prototype() => 
            new MainScreenPresenter();

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