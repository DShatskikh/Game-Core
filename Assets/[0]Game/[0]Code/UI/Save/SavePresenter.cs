using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game
{
    public sealed class SavePresenter : IScreenPresenter
    {
        private SaveView _view;
        private ScreenManager _screenManager;
        private GameStateController _gameStateController;

        [Inject]
        private void Construct(SaveView view, ScreenManager screenManager, GameStateController gameStateController)
        {
            _view = view;
            _screenManager = screenManager;
            _gameStateController = gameStateController;

            _gameStateController.OpenDialog();
            
            //_view.GetButton.onClick.AddListener(OnClick);
            //EventSystem.current.SetSelectedGameObject(_view.GetButton.gameObject);
        }
        
        public IScreenPresenter Prototype() => 
            new GameOverPresenter();

        public void Destroy()
        {
            Object.Destroy(_view.gameObject);
        }
    }
}