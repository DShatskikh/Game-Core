using UnityEngine;
using Zenject;

namespace Game
{
    public class BattleInstaller : MonoInstaller
    {
        [SerializeField]
        private Arena _arena;
        
        public override void InstallBindings()
        {
            var battleController = GetComponent<BattleController>();
            
            Container.Bind<IBattleController>().FromInstance(battleController).AsSingle();
            Container.Bind<Arena>().FromInstance(_arena).AsSingle();

            foreach (var monoBehaviour in GetComponentsInChildren<MonoBehaviour>(true)) 
                Container.Inject(monoBehaviour);
        }
    }
}