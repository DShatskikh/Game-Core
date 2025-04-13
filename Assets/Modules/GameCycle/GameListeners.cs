namespace Game
{
    public interface IGameListener { }
        
    public interface IGameStartListener : IGameListener
    {
        void OnStartGame();
    }

    public interface IGameTransitionListener : IGameListener
    {
        void OnStartTransition();
        void OnEndTransition();
    }

    public interface IGameShopListener : IGameListener
    {
        void OnOpenShop();
        void OnCloseShop();
    }

    public interface IGameADSListener : IGameListener
    {
        void OnShowADS();
        void OnHideADS();
    }
    
    public interface IGameCutsceneListener : IGameListener
    {
        void OnShowCutscene();
        void OnHideCutscene();
    }
    
    public interface IGameEnderChestListener : IGameListener
    {
        void OnOpenEnderChest();
        void OnCloseEnderChest();
    }
    
    public interface IGameBattleListener : IGameListener
    {
        void OnOpenBattle();
        void OnCloseBattle();
    }
    
    public interface IGameGameOvertListener : IGameListener
    {
        void OnGameOver();
    }
}