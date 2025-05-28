using UnityEngine;
using Zenject;

namespace Game
{
    // Добавляет прогресс битвы со временем
    public sealed class TimeBasedTurnBooster : MonoBehaviour
    {
        private const float START_TIMER = 0.5f;
        
        private bool _isActive;
        private float _time = START_TIMER;
        private TurnProgressStorage _turnProgressStorage;

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
                _turnProgressStorage.AddBattleProgress(3);
            }
        }

        public void ToggleActivate(bool value) => 
            _isActive = value;
    }
}