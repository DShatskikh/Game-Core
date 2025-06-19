using UnityEngine;
using Zenject;

namespace Game.Test
{
    public sealed class TestSpawnAttack : MonoBehaviour
    {
        [SerializeField]
        private Attack _attack;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private Arena _arena;
        
        private DiContainer _diContainer;
        private HeartModeService _heartModeService;
        private TimeBasedTurnBooster _timeBasedTurnBooster;
        private Heart _heart;

        [Inject]
        private void Construct(DiContainer diContainer, HeartModeService heartModeService,
            TimeBasedTurnBooster timeBasedTurnBooster, Heart heart)
        {
            _diContainer = diContainer;
            _heartModeService = heartModeService;
            _timeBasedTurnBooster = timeBasedTurnBooster;
            _heart = heart;
        }
        
        private void Start()
        {
            _arena.SetSize(_attack.GetSizeArena);
            var attack = Instantiate(_attack, _container);
            _heartModeService.SetMode(attack.GetStartHeartMode);
            _timeBasedTurnBooster.SetAddedProgress(attack.GetTurnAddedProgress);
            _heart.SetAddedProgress(attack.GetShieldAddedProgress);

            foreach (var monoBehaviour in attack.GetComponentsInChildren<MonoBehaviour>())
            {
                _diContainer.Inject(monoBehaviour);
            }
        }
    }
}