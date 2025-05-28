using System;
using UnityEngine;

namespace Game
{
    public sealed class TutorialState
    {
        private readonly MainRepositoryStorage _mainRepositoryStorage;

        public event Action<TutorialStep> OnStepStarted;
        public event Action<TutorialStep> OnStepFinished;
        public event Action OnCompleted;

        public bool IsCompleted { get; private set; }
        public TutorialStep CurrentStep { get; private set; }

        [Serializable]
        public struct Data
        {
            public TutorialStep CurrentStep;
            public bool IsCompleted;
        }
        
        public TutorialState(MainRepositoryStorage mainRepositoryStorage)
        {
            _mainRepositoryStorage = mainRepositoryStorage;
        }

        public void Start()
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.TUTORIAL, out Data data))
            {
                IsCompleted = data.IsCompleted;
                CurrentStep = data.CurrentStep; 
            }
            
            if (CurrentStep == TutorialStep.START)
            {
                NextStep();
                return;
            }
            
            OnStepStarted?.Invoke(CurrentStep);
        }
        
        public void NextStep()
        {
            CurrentStep++;
            Debug.Log(CurrentStep);

            if (CurrentStep == TutorialStep.END)
            {
                IsCompleted = true;
                OnCompleted?.Invoke();
            }
            else
            {
                OnStepStarted?.Invoke(CurrentStep);
            }

            _mainRepositoryStorage.Set(SaveConstants.TUTORIAL, GetData());
        }

        public void FinishStep(bool moveNext = true)
        {
            OnStepFinished?.Invoke(CurrentStep);
            
            if (moveNext)
                NextStep();
        }

        private Data GetData()
        {
            return new Data()
            {
                IsCompleted = IsCompleted,
                CurrentStep = CurrentStep
            };
        }
    }
}