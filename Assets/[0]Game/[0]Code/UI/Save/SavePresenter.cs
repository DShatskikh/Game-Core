using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    // Логика окна сохранения
    [Serializable]
    public sealed class SavePresenter : IScreenPresenter
    {
        private SaveView _view;
        private ScreenManager _screenManager;
        private GameStateController _gameStateController;
        private LevelService _levelService;
        private SessionTimeSystem _sessionTimeSystem;
        private MainRepositoryStorage _mainRepositoryStorage;

        [Inject]
        private void Construct(SaveView view, ScreenManager screenManager, GameStateController gameStateController, 
            LevelService levelService, SessionTimeSystem sessionTimeSystem, MainRepositoryStorage mainRepositoryStorage)
        {
            _view = view;
            _screenManager = screenManager;
            _gameStateController = gameStateController;
            _levelService = levelService;
            _sessionTimeSystem = sessionTimeSystem;
            _mainRepositoryStorage = mainRepositoryStorage;
            
            _view.GetSaveButton.onClick.AddListener(OnClickSaveButton);
            _view.GetReturnButton.onClick.AddListener(OnClickReturnButton);
            EventSystem.current.SetSelectedGameObject(_view.GetSaveButton.gameObject);

            _view.GetInfoLabel.text = $"Нубик УР{_levelService.GetPreviousLv} {_sessionTimeSystem.GetTextTime()}";
            
            WaitOpenCutscene().Forget();
        }

        private async UniTask WaitOpenCutscene()
        {
            await UniTask.DelayFrame(1);
            _gameStateController.OpenCutscene();
        }

        private void OnClickSaveButton()
        {
            _levelService.SaveGame();
            WaitSaveAnimation().Forget();
        }

        private void OnClickReturnButton()
        {
            _screenManager.Close(ScreensEnum.SAVE);
        }

        private async UniTask WaitSaveAnimation()
        {
            _sessionTimeSystem.Save();
            _mainRepositoryStorage.Save();
            _view.GetInfoLabel.text = $"Нубик УР{_levelService.GetLv} {_sessionTimeSystem.GetTextTime()}";
            
            _view.GetSaveButton.gameObject.SetActive(false);
            _view.GetReturnButton.gameObject.SetActive(false);
            _view.GetSavedLabel.gameObject.SetActive(true);
            _view.GetInfoLabel.color = Color.yellow;
            _view.PlaySound();
            
            await UniTask.WaitForSeconds(2);
            
            _screenManager.Close(ScreensEnum.SAVE);
        }

        public IScreenPresenter Prototype() => 
            new SavePresenter();

        public void Destroy()
        {
            PlayerOn().Forget();
            Object.Destroy(_view.gameObject);
        }

        private async UniTask PlayerOn()
        {
            await UniTask.WaitForSeconds(0.5f);
            _gameStateController.CloseCutscene();
        }
    }
}