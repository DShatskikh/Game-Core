using System.Collections;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class TransitionService
    {
        private GameStateController _gameStateController;
        private TransitionScreen _transitionScreen;
        private LocationsManager _locationsManager;

        [Inject]
        private void Construct(GameStateController gameStateController, TransitionScreen transitionScreen, LocationsManager locationsManager)
        {
            _gameStateController = gameStateController;
            _transitionScreen = transitionScreen;
            _locationsManager = locationsManager;
        }

        public void Transition(string nextLocationIndex, int pointIndex, AudioClip audioClip = null) => 
            CoroutineRunner.Instance.StartCoroutine(AwaitTransition(nextLocationIndex, pointIndex, audioClip));

        public IEnumerator AwaitTransition(string nextLocationIndex, int pointIndex, AudioClip audioClip = null)
        {
            _gameStateController.StartTransition();
            yield return _transitionScreen.AwaitShow();
            
            if (audioClip)
                SoundPlayer.Play(audioClip);
            
            _locationsManager.SwitchLocation(nextLocationIndex, pointIndex);
            yield return _transitionScreen.AwaitHide();
            _gameStateController.EndTransition();
        }
    }
}