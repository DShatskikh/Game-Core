using QFSW.QC;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

#if YandexGamesPlatform_yg
using YG;
#endif

namespace Game
{
    // Инсталлер для всего приложения
    public class GlobalInstaller : MonoInstaller
    {
        [Header("Services")]
        [SerializeField]
        private PlayerInput _playerInput;
        
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

        [SerializeField]
        private QuantumConsole _quantumConsole;

        private IAssetLoader _assetLoader;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerInput>().FromInstance(_playerInput).AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateController>().AsCached();
            Container.Bind<ConsoleToggleHandler>().AsSingle().WithArguments(_quantumConsole).NonLazy();
            Container.Bind<ConsoleCommandRegistry>().AsSingle().NonLazy();
            Container.Bind<INextButton>().FromInstance(_nextButton).AsSingle().NonLazy();
            Container.Bind<VolumeSliderPresenter>().AsSingle().WithArguments(_volumeSlider).NonLazy();
            Container.Bind<VolumeService>().AsSingle().NonLazy();
            Container.Bind<ScreenManager>().AsSingle().WithArguments(_screenConfig, _ui).NonLazy();
            Container.Bind<CoroutineRunner>().FromInstance(_coroutineRunner).AsSingle().NonLazy();
            Container.Bind<HeartModeService>().AsSingle().WithArguments(_heartIcons).NonLazy();
            Container.Bind<LuaCommandRegister>().AsSingle().NonLazy();

            var mainRepositoryStorage = new MainRepositoryStorage();
            Container.Bind<IGameRepositoryStorage>().FromInstance(mainRepositoryStorage).AsSingle().NonLazy();
            Container.Bind<SettingsRepositoryStorage>().AsSingle().NonLazy();
            Container.Bind<SessionTimeSystem>().AsSingle().NonLazy();

            _assetLoader = new AssetLoader();
            Container.Bind<IAssetLoader>().FromInstance(_assetLoader).AsSingle().NonLazy();

#if YandexGamesPlatform_yg // Собираем под Яндекс игры
    Container.Bind<IAnalyticsService>().To<YandexGamesAnalytics>().AsSingle().NonLazy();
    Container.Bind<IADSService>().To<YandexGamesADS>().AsSingle().NonLazy();
    Container.Bind<IPurchaseService>().To<YandexGamesPurchase>().AsSingle().NonLazy();
#elif RUSTORE // Собираем под Rustore
    Container.Bind<IAnalyticsService>().To<AppMetricaAnalytics>().AsSingle().NonLazy();
    Container.Bind<IADSService>().To<YandexMobileADS>().AsSingle().NonLazy();
    Container.Bind<IPurchaseService>().To<RustorePurchase>().AsSingle().NonLazy();
#else // Собираем под ПК
    Container.Bind<IAnalyticsService>().To<EmptyAnalytics>().AsSingle().NonLazy();
    Container.Bind<IADSService>().To<EmptyADS>().AsSingle().NonLazy();
    Container.Bind<IPurchaseService>().To<EmptyPurchase>().AsSingle().NonLazy();
#endif
            
            mainRepositoryStorage.Load();
        }
    }
}