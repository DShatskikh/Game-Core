namespace Game
{
    public enum GameState : byte
    {
        OFF = 0,
        PLAYING = 1,
        PAUSED = 2,
        FINISHED = 3,
        MAIN_MENU = 4,
        TRANSITION = 5,
        SHOP =6,
        ADS = 7,
        DIALOGUE = 8,
        ENDER_CHEST = 9,
        BATTLE = 10,
        GAME_OVER = 11,
    }
}