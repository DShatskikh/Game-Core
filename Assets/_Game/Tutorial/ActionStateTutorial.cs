using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game
{
    public sealed class ActionStateTutorial : MonoBehaviour
    {
        [SerializeField]
        private TutorialStep _tutorialStep;

        [SerializeField]
        private UnityEvent _startEvent;
        
        [SerializeField]
        private UnityEvent _endEvent;
        
        [Inject]
        private TutorialState _tutorialState;

        private void Start()
        {
            if (_tutorialState.CurrentStep == _tutorialStep)
            {
                _startEvent.Invoke();
            }
            else
            {
                _endEvent.Invoke();
                _tutorialState.OnStepStarted += TutorialStateOnOnStepStarted;
                _tutorialState.OnStepFinished += TutorialStateOnOnStepFinished;
            }
        }

        private void OnDestroy()
        {
            _tutorialState.OnStepStarted -= TutorialStateOnOnStepStarted;
            _tutorialState.OnStepFinished -= TutorialStateOnOnStepFinished;
        }

        private void TutorialStateOnOnStepStarted(TutorialStep obj)
        {
            if (obj != _tutorialStep)
                return;
            
            _startEvent.Invoke();
        }

        private void TutorialStateOnOnStepFinished(TutorialStep obj)
        {
            if (obj != _tutorialStep)
                return;
            
            _endEvent.Invoke();
            _tutorialState.OnStepStarted -= TutorialStateOnOnStepStarted;
            _tutorialState.OnStepFinished -= TutorialStateOnOnStepFinished;
        }
    }
}