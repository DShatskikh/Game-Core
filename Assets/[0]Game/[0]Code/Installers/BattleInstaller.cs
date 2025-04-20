using System;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Game
{
    public class BattleInstaller : MonoInstaller
    {
        [SerializeField]
        private BattleView _view;
        
        [SerializeField]
        private ShopButton _buttonPrefab;

        [SerializeField]
        private EnemyBattleButton _enemyBattleButton;
        
        [SerializeField]
        private Arena _arena;

        [SerializeField]
        private Heart _heart;
        
        [SerializeField]
        private BattlePoints _points;
        
        [SerializeField]
        private CinemachineVirtualCamera _virtualCamera;

        [SerializeField]
        private TimeBasedTurnBooster _timeBasedTurnBooster;
        
        public Func<BattleControllerBase> CreatePresenterCommand;

        public override void InstallBindings()
        {
            Container.Bind<Arena>().FromInstance(_arena).AsSingle();
            Container.Bind<BattleView>().FromInstance(_view).AsSingle();
            Container.Bind<ShopButton>().FromInstance(_buttonPrefab).AsSingle();
            Container.Bind<EnemyBattleButton>().FromInstance(_enemyBattleButton).AsSingle();
            Container.Bind<BattlePoints>().FromInstance(_points).AsSingle();
            Container.Bind<Heart>().FromInstance(_heart).AsSingle();
            Container.Bind<CinemachineVirtualCamera>().FromInstance(_virtualCamera).AsSingle();
            Container.Bind<TurnProgressStorage>().AsSingle();
            Container.BindInstance(_timeBasedTurnBooster).AsSingle().NonLazy();
            
            var presenter = CreatePresenterCommand?.Invoke();
            
            Container.Bind<BattleControllerBase>().FromInstance(presenter).AsSingle();
            Container.Bind<IGameGameOvertListener>().FromInstance(presenter).AsCached();

            foreach (var monoBehaviour in GetComponentsInChildren<MonoBehaviour>(true)) 
                Container.Inject(monoBehaviour);
        }

        [Inject]
        private void Construct(GameStateController gameStateController)
        {
            gameStateController.StartBattle();
        }
    }
}