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

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        private void Start()
        {
            _arena.SetSize(_attack.GetSizeArena);
            var attack = Instantiate(_attack, _container);

            foreach (var monoBehaviour in attack.GetComponentsInChildren<MonoBehaviour>())
            {
                _diContainer.Inject(monoBehaviour);
            }
        }
    }
}