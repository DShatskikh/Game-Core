namespace Game
{
    public interface IEnemy
    {
        Attack[] GetAttacks { get; }
        public BattleMessageBox GetMessageBox { get; }
    }
}