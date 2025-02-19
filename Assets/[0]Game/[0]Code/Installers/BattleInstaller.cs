using System;
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
        private Arena _arena;

        [SerializeField]
        private Heart _heart;
        
        [SerializeField]
        private BattlePoints _points;
        
        public Func<BattlePresenterBase> CreatePresenterCommand;

        public override void InstallBindings()
        {
            Container.Bind<Arena>().FromInstance(_arena).AsSingle();
            Container.Bind<BattleView>().FromInstance(_view).AsSingle();
            Container.Bind<ShopButton>().FromInstance(_buttonPrefab).AsSingle();
            Container.Bind<BattlePoints>().FromInstance(_points).AsSingle();
            Container.Bind<Heart>().FromInstance(_heart).AsSingle();
            
            var presenter = CreatePresenterCommand?.Invoke();
            
            Container.Bind<BattlePresenterBase>().FromInstance(presenter).AsSingle();

            foreach (var monoBehaviour in GetComponentsInChildren<MonoBehaviour>(true)) 
                Container.Inject(monoBehaviour);
        }
    }
}