using UnityEngine;
using Zenject;

namespace Game
{
    public abstract class BaseStepController : MonoBehaviour
    {
        private protected TutorialState  _tutorialState;

        private protected abstract TutorialStep _step { get; }

        [Inject]
        private void Construct(TutorialState tutorialState)
        {
            _tutorialState = tutorialState;
            
            _tutorialState.OnStepStarted += OnStart;
            _tutorialState.OnStepFinished += OnFinish;
        }

        private void OnDestroy()
        {
            _tutorialState.OnStepStarted -= OnStart;
            _tutorialState.OnStepFinished -= OnFinish;
        }

        private void OnStart(TutorialStep step)
        {
            if (step != _step)
                return;

            OnStepStarted();
        }

        private void OnFinish(TutorialStep step)
        {
            if (step != _step)
                return;

            OnStepFinished();
        }
        
        private protected abstract void OnStepStarted();
        private protected abstract void OnStepFinished();
    }
}