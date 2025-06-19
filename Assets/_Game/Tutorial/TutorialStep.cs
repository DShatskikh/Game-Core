namespace Game
{
    public enum TutorialStep : byte
    {
        START = 0,
        START_MOVE = 1, // Показываем что можно ходить
        BATTLE_BANANA = 2, // Сражаемся с 1 противником (Идем на спавн 2, затем на арену, стрелочка на противника)
        MOVE_TO_SHOP_NO_MONEY_BUY_SWORD = 3, // Идем в магазин, но у нас нет денег (Разговор с Нотчем)
        MOVE_MOB_ARENA = 4, // Идем на моб арену чтобы заработать денег
        MOVE_TO_SHOP_BUY_SWORD = 5, // Идем с деньгами покупать меч
        BATTLE_DIMAS = 6, // Сражаемся с Димасом
        WAIT_WIN_HEROBRINE = 7, // Ждем пока не победим Херобрина
        MOVE_TO_LOBBY = 8, // Идем в портал
        END = 9
    }
}