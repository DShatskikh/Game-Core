namespace Game
{
    // Список действий игрока во время битвы
    public enum BattleActionType : byte
    {
        Attack,
        AttackMiss,
        Item,
        Action,
        Mercy,
        NoAction,
    }
}