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
        
        private DiContainer _diContainer;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        private void Start()
        {
            var attack = Instantiate(_attack, _container);

            foreach (var monoBehaviour in attack.GetComponentsInChildren<MonoBehaviour>())
            {
                _diContainer.Inject(monoBehaviour);
            }
        }
    }
}