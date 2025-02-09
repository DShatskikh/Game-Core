using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game
{
    public class GlobalInstaller : MonoInstaller
    {
        [Header("SoundServices")]
        [SerializeField]
        private AudioSource _musicSource;
        
        [SerializeField]
        private AudioSource _soundSource_1;
        
        [SerializeField]
        private AudioSource _soundSource_2;
        
        [Header("Services")]
        [SerializeField]
        private PlayerInput _playerInput;

        [SerializeField]
        private AssetProvider _assetProvider;
        
        public override void InstallBindings()
        {
            _assetProvider.Init();
            Container.Bind<PlayerInput>().FromInstance(_playerInput).AsSingle();
            
            Container.Bind<VolumeService>().AsSingle();
            
            var musicPlayer = new MusicPlayer(_musicSource);
            var soundPlayer = new SoundPlayer(_soundSource_1, _soundSource_2);
            
            Container.BindInterfacesAndSelfTo<GameStateController>().AsCached();
            Container.Bind<ConsoleToggleHandler>().AsSingle().WithArguments(AssetProvider.Instance.QuantumConsole).NonLazy();
            Container.Bind<ConsoleCommandRegistry>().AsSingle().NonLazy();
        }
    }
}