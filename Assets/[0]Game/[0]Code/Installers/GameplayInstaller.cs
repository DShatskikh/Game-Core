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
        private Transform _screens;
        
        public override void InstallBindings()
        {
            Container.Bind<CinemachineConfiner2D>().FromInstance(_cinemachineConfiner).AsSingle();
            Container.BindInterfacesAndSelfTo<Player>().FromInstance(_player).AsCached();
            Container.BindInterfacesAndSelfTo<EnderChestToggleHandler>().AsCached().WithArguments(_screens, AssetProvider.Instance.EnderChestScreen).NonLazy();
        }
    }
}