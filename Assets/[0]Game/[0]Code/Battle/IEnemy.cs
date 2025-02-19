namespace Game
{
    public interface IEnemy
    {
        Attack[] GetAttacks { get; }
        public BattleMessageBox GetMessageBox { get; }
        public int GetHealth { get; }
        public void Damage(int damage);
    }
}