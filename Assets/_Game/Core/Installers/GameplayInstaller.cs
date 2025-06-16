using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Game
{
    // Инсталлер для основной игры
    public class GameplayInstaller : MonoInstaller
    {
        public static DiContainer GetContainer;
        
        [SerializeField]
        private CinemachineConfiner2D _cinemachineConfiner;

        [SerializeField]
        private Player _player;

        // Регистрация сервисов
        public override void InstallBindings()
        {
            GetContainer = Container;
            
            Container.Bind<CinemachineConfiner2D>().FromInstance(_cinemachineConfiner).AsSingle();
            Container.BindInterfacesAndSelfTo<Player>().FromInstance(_player).AsCached();
            Container.Bind<MainInventory>().AsSingle().NonLazy();
            Container.Bind<WalletService>().AsSingle().NonLazy();
            Container.Bind<LocationsManager>().AsSingle().WithArguments(Resources.LoadAll<Location>("Locations")).NonLazy();
            Container.BindFactory<Location, Location, Location.Factory>();
            Container.Bind<TransitionService>().AsSingle().NonLazy();
            Container.Bind<LevelService>().AsSingle().NonLazy();
            Container.Bind<HealthService>().AsSingle().NonLazy();
            Container.Bind<TutorialState>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<MainScreenHandler>().AsCached().NonLazy();
        }
    }
}