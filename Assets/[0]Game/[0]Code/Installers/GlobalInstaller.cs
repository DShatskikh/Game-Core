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
            Container.Bind<MainRepositoryStorage>().AsSingle().NonLazy();
            Container.Bind<SettingsRepositoryStorage>().AsSingle().NonLazy();
            Container.Bind<SessionTimeSystem>().AsSingle().NonLazy();

#if YandexGamesPlatform_yg // Собираем под Яндекс игры
    YG2.StartInit();
    Container.Bind<IAnalyticsService>().To<YandexAnalytics>().AsSingle().NonLazy();
    Container.Bind<IADSService>().To<YandexADS>().AsSingle().NonLazy();
    Container.Bind<IPurchaseService>().To<YandexPurchase>().AsSingle().NonLazy();
#elif UNITY_ANDROID // Собираем под Rustore
    Container.Bind<IAnalyticsService>().To<YandexMetricaAnalytics>().AsSingle().NonLazy();
    Container.Bind<IADSService>().To<RustoreADS>().AsSingle().NonLazy();
    Container.Bind<IPurchaseService>().To<RustorePurchase>().AsSingle().NonLazy();
#else // Собираем под ПК
    Container.Bind<IAnalyticsService>().To<EmptyAnalytics>().AsSingle().NonLazy();
    Container.Bind<IADSService>().To<EmptyADS>().AsSingle().NonLazy();
    Container.Bind<IPurchaseService>().To<EmptyPurchase>().AsSingle().NonLazy();
#endif
        }
    }
}