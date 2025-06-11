using UnityEngine;
using Zenject;

namespace Game.Test
{
    public sealed class AttackTestInstaller : MonoInstaller
    {
        [SerializeField]
        private Heart _heart;
        
        [SerializeField]
        private Sprite[] _heartIcons;

        [SerializeField]
        private TimeBasedTurnBooster _timeBasedTurnBooster;

        [SerializeField]
        private Arena _arena;

        public override void InstallBindings()
        {
            Container.Bind<Heart>().FromInstance(_heart).AsSingle().NonLazy();
            Container.Bind<HeartModeService>().AsSingle().WithArguments(_heartIcons).NonLazy();
            Container.Bind<HealthService>().AsSingle().NonLazy();
            Container.Bind<TimeBasedTurnBooster>().FromInstance(_timeBasedTurnBooster).AsSingle().NonLazy();
            Container.Bind<Arena>().FromInstance(_arena).AsSingle().NonLazy();
            Container.Bind<TurnProgressStorage>().AsSingle().NonLazy();
        }
    }
}