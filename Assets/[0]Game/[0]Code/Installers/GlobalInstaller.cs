using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game
{
    public class GlobalInstaller : MonoInstaller
    {
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
        private ScreenConfig _screenConfig;

        [SerializeField]
        private Transform _ui;

        [SerializeField]
        private CoroutineRunner _coroutineRunner;
        
        [SerializeField]
        private Sprite[] _heartIcons;
        
        public override void InstallBindings()
        {
            _assetProvider.Init();
            Container.Bind<PlayerInput>().FromInstance(_playerInput).AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateController>().AsCached();
            Container.Bind<ConsoleToggleHandler>().AsSingle().WithArguments(AssetProvider.Instance.QuantumConsole).NonLazy();
            Container.Bind<ConsoleCommandRegistry>().AsSingle().NonLazy();
            Container.Bind<INextButton>().FromInstance(_nextButton).AsSingle().NonLazy();
            Container.Bind<VolumeSliderPresenter>().AsSingle().WithArguments(_volumeSlider).NonLazy();
            Container.Bind<VolumeService>().AsSingle().NonLazy();
            Container.Bind<ScreenManager>().AsSingle().WithArguments(_screenConfig, _ui).NonLazy();
            Container.Bind<CoroutineRunner>().FromInstance(_coroutineRunner).AsSingle().NonLazy();
            Container.Bind<HeartModeService>().AsSingle().WithArguments(_heartIcons).NonLazy();
        }
    }
}