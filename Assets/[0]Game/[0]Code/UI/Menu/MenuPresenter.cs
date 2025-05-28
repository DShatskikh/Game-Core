using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    // Логика меню
    [Serializable]
    public sealed class MenuPresenter : IScreenPresenter
    {
        private MenuView _view;
        private GameStateController _gameStateController;
        private ScreenManager _screenManager;
        private DiContainer _container;

        [Inject]
        private void Construct(MenuView menuView, DiContainer container, GameStateController gameStateController, ScreenManager screenManager)
        {
            _view = menuView;
            _container = container;
            _gameStateController = gameStateController;
            _screenManager = screenManager;
            
            _gameStateController.OpenCutscene();
            
            _view.GetContinueButton.onClick.AddListener(OnContinueClicked);
            _view.GetInventoryButton.onClick.AddListener(OnInventoryClicked);
            _view.GetStatsButton.onClick.AddListener(OnStatsClicked);
            _view.GetMenuButton.onClick.AddListener(OnMenuClicked);
            EventSystem.current.SetSelectedGameObject(_view.GetContinueButton.gameObject);
            OnStatsClicked();
        }

        public IScreenPresenter Prototype()
        {
            return new MenuPresenter();
        }

        public void Destroy()
        {
            _view.GetContinueButton.onClick.RemoveListener(OnContinueClicked);
            _view.GetContinueButton.onClick.RemoveListener(OnMenuClicked);
            
            _gameStateController.CloseCutscene();
            Object.Destroy(_view.gameObject);
        }

        private void OnContinueClicked()
        {
            _screenManager.Open(ScreensEnum.MAIN, _container);
            _screenManager.Close(ScreensEnum.MENU);
        }

        private void OnMenuClicked()
        {
            _screenManager.Close(ScreensEnum.MENU);
            _screenManager.Close(ScreensEnum.MAIN);
            SceneManager.LoadScene(0);
        }

        private void OnInventoryClicked()
        {
            _view.ToggleStats(false);
            _view.ToggleInventory(true);
        }

        private void OnStatsClicked()
        {
            _view.ToggleStats(true);
            _view.ToggleInventory(false);
        }
    }
}