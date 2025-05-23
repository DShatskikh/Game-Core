using System.Collections;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class TransitionService
    {
        private GameStateController _gameStateController;
        private LocationsManager _locationsManager;
        private ScreenManager _screenManager;

        [Inject]
        private void Construct(GameStateController gameStateController, 
            LocationsManager locationsManager, 
            ScreenManager screenManager)
        {
            _gameStateController = gameStateController;
            _locationsManager = locationsManager;
            _screenManager = screenManager;
        }

        public void Transition(string id, int pointIndex, AudioClip audioClip = null) => 
            CoroutineRunner.Instance.StartCoroutine(AwaitTransition(id, pointIndex, audioClip));

        public IEnumerator AwaitTransition(string id, int pointIndex, AudioClip audioClip = null)
        {
            var transitionScreen = (TransitionPresenter)_screenManager.Open(ScreensEnum.TRANSITION);
            
            _gameStateController.StartTransition();
            transitionScreen.Show();
            yield return transitionScreen.AwaitShow();
            
           // if (audioClip)
                //SoundPlayer.Play(audioClip);
            
            _locationsManager.SwitchLocation(id, pointIndex);
            _gameStateController.DestroyRemovedListeners();
            yield return transitionScreen.AwaitHide();
            _gameStateController.EndTransition();
            _screenManager.Close(ScreensEnum.TRANSITION);
        }
    }
}