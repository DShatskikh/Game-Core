using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
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
        private PlayerInput _playerInput;

        public override void InstallBindings()
        {
            Container.Bind<CinemachineConfiner2D>().FromInstance(_cinemachineConfiner).AsSingle();
            Container.Bind<PlayerInput>().FromInstance(_playerInput).AsSingle();
            Container.BindInterfacesTo<Player>().FromInstance(_player).AsSingle();
        }
    }
}