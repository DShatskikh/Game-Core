using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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

        [SerializeField]
        private NextButton _nextButton;

        [SerializeField]
        private VolumeSliderView _volumeSlider;

        [SerializeField]
        private AudioMixer _audioMixer;
        
        public override void InstallBindings()
        {
            _assetProvider.Init();
            Container.Bind<PlayerInput>().FromInstance(_playerInput).AsSingle();
            
            var musicPlayer = new MusicPlayer(_musicSource);
            var soundPlayer = new SoundPlayer(_soundSource_1, _soundSource_2);
            
            Container.BindInterfacesAndSelfTo<GameStateController>().AsCached();
            Container.Bind<ConsoleToggleHandler>().AsSingle().WithArguments(AssetProvider.Instance.QuantumConsole).NonLazy();
            Container.Bind<ConsoleCommandRegistry>().AsSingle().NonLazy();
            Container.Bind<INextButton>().FromInstance(_nextButton).AsSingle().NonLazy();
            Container.Bind<VolumeSliderPresenter>().AsSingle().WithArguments(_volumeSlider).NonLazy();
            Container.Bind<VolumeService>().AsSingle().WithArguments(_audioMixer).NonLazy();
        }
    }
}