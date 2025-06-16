using Zenject;

namespace Game
{
    // Обработчик который показывает и скрывает основное окно в зависимости от состояния игры
    public sealed class MainScreenHandler : IGameStartListener, IGameCutsceneListener, IGameBattleListener, 
        IGameShopListener, IGameTransitionListener, IGameADSListener
    {
        private readonly ScreenManager _screenManager;
        private readonly DiContainer _container;

        private MainScreenHandler(ScreenManager screenManager, DiContainer container)
        {
            _screenManager = screenManager;
            _container = container;
        }

        public void OnStartGame() => 
            ToggleScreen(true);

        public void OnShowCutscene() => 
            ToggleScreen(false);

        public void OnHideCutscene() => 
            ToggleScreen(true);

        public void OnOpenBattle() => 
            ToggleScreen(false);

        public void OnCloseBattle() => 
            ToggleScreen(true);

        public void OnOpenShop() => 
            ToggleScreen(false);

        public void OnCloseShop() => 
            ToggleScreen(true);

        public void OnStartTransition() => 
            ToggleScreen(false);

        public void OnEndTransition() => 
            ToggleScreen(true);

        public void OnShowADS() => 
            ToggleScreen(false);

        public void OnHideADS() => 
            ToggleScreen(true);

        private void ToggleScreen(bool value)
        {
            if (value)
            {
                _screenManager.Open(ScreensEnum.MAIN, _container);
            }
            else
            {
                _screenManager.Close(ScreensEnum.MAIN);
            }
        }
    }
}