using I2.Loc;

namespace Game
{
    public interface IEnemy
    {
        LocalizedString Name { get; }
        Attack[] Attacks { get; }
        BattleMessageBox MessageBox { get; }
        ActionBattle[] Actions { get; }
        int Health { get; }
        int MaxHealth { get; }
        int Mercy { get; set; }
        bool IsMercy { get; set; }
        void Damage(int damage);
        void Death(int damage);
    }
}