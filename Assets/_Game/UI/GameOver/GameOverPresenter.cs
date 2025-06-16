using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    // Логика окна прогрыша
    public sealed class GameOverPresenter : IScreenPresenter
    {
        private GameOverView _view;
        private ScreenManager _screenManager;

        [Inject]
        private void Construct(GameOverView view, ScreenManager screenManager)
        {
            _view = view;
            _screenManager = screenManager;
            
            _view.GetButton.onClick.AddListener(OnClick);
            EventSystem.current.SetSelectedGameObject(_view.GetButton.gameObject);
        }

        public IScreenPresenter Prototype() => 
            new GameOverPresenter();

        public void Destroy()
        {
            Object.Destroy(_view.gameObject);
        }

        public void SetMessage(string message)
        {
            _view.SetLabel(message);
        }

        private void OnClick()
        {
            _screenManager.Close(ScreensEnum.GAME_OVER);
            SceneManager.LoadScene(1);
        }
    }
}