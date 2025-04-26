using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField]
        private CinemachineConfiner2D _cinemachineConfiner;

        [SerializeField]
        private Player _player;

        [SerializeField]
        private Sprite[] _heartIcons;
        
        public override void InstallBindings()
        {
            Container.Bind<CinemachineConfiner2D>().FromInstance(_cinemachineConfiner).AsSingle();
            Container.BindInterfacesAndSelfTo<Player>().FromInstance(_player).AsCached();
            Container.Bind<MainInventory>().AsSingle().NonLazy();
            Container.Bind<WalletService>().AsSingle().NonLazy();
            Container.Bind<LocationsManager>().AsSingle().WithArguments(Resources.LoadAll<Location>("")).NonLazy();
            Container.BindFactory<Location, Location, Location.Factory>();
            Container.Bind<TransitionService>().AsSingle().NonLazy();
            Container.Bind<HeartModeService>().AsSingle().WithArguments(_heartIcons).NonLazy();
            
            Container.BindInterfacesAndSelfTo<MainScreenHandler>().AsCached().NonLazy();
        }
    }
}