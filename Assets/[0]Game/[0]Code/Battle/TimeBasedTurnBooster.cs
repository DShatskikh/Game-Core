using UnityEngine;
using Zenject;

namespace Game
{
    // Добавляет прогресс битвы со временем
    public sealed class TimeBasedTurnBooster : MonoBehaviour
    {
        private const int START_ADDED_PROGRESS = 3;
        private const float START_TIMER = 0.5f;
        
        private bool _isActive;
        private float _time = START_TIMER;
        private TurnProgressStorage _turnProgressStorage;
        private int _addedProgress = START_ADDED_PROGRESS;

        [Inject]
        private void Construct(TurnProgressStorage turnProgressStorage)
        {
            _turnProgressStorage = turnProgressStorage;
        }
        
        public void Update()
        {
            if (!_isActive)
                return;
            
            _time -= Time.deltaTime;

            if (_time < 0)
            {
                _time = START_TIMER;
                _turnProgressStorage.AddBattleProgress(_addedProgress);
            }
        }

        public void SetAddedProgress(int addedProgress) => 
            _addedProgress = addedProgress;

        public void ResetAddedProgress() => 
            _addedProgress = START_ADDED_PROGRESS;
        
        public void ToggleActivate(bool value)
        {
            _isActive = value;
            ResetAddedProgress();
        }
    }
}