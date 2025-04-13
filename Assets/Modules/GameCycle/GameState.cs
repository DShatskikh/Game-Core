namespace Game
{
    public enum GameState : byte
    {
        OFF = 0,
        MAIN_MENU = 1,
        PLAYING = 2,
        CUTSCENE = 3,
        TRANSITION = 4,
        BATTLE = 5,
        GAME_OVER = 6,
        SHOP =7,
        ADS = 8,
        ENDER_CHEST = 9,
    }
}