using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game
{
    // Базовый класс для начала битвы
    public abstract class StarterBattle<T,TFactory> : StarterBattleBase where T : BattleControllerBase where TFactory : PlaceholderFactory<T>
    {
        [SerializeField]
        private protected BattleInstaller _installerPrefab;

        private DiContainer _diContainer;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        private void Start()
        {
            Open();
        }

        [Button]
        private void Open()
        {
            StartBattle();
        }

        public override void StartBattle()
        {
            OnStartBattle();
            gameObject.SetActive(true);

            var installer = Instantiate(_installerPrefab, transform.position.SetZ(0), Quaternion.identity);
            var subContainer = _diContainer.CreateSubContainer();
            subContainer.Inject(installer);

            installer.CreatePresenterCommand = () =>
            {
                Binding(subContainer);

                var factory = subContainer.TryResolve<TFactory>();
                subContainer.Unbind<TFactory>();
                return factory.Create();
            };

            installer.InstallBindings();
        }

        private protected virtual void OnStartBattle() { }
    }
}