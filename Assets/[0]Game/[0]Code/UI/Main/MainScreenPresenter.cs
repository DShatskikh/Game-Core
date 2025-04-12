using System;
using Zenject;

namespace Game
{
    [Serializable]
    public sealed class MainScreenPresenter : IScreenPresenter, IGameDialogueListener, IGameBattleListener
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
            
        }
        
        void IGameDialogueListener.OnShowDialogue() => 
            _view.ToggleActivate(false);
        
        void IGameDialogueListener.OnHideDialogue() => 
            _view.ToggleActivate(true);
        
        void IGameBattleListener.OnOpenBattle() => 
            _view.ToggleActivate(false);
        
        void IGameBattleListener.OnCloseBattle() => 
            _view.ToggleActivate(true);
    }
}