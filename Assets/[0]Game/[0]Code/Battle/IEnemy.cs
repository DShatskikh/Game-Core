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
        bool CanMercy { get; }
        bool IsMercy { get; set; }
        int GetOP { get; }
        int GetMoney { get; }
        string GetID { get; }
        void Damage(int damage);
        void Death(int damage);
        void Spared();
        string GetReaction(BattleActionType actionType, Item item = null);
        string GetDeathReaction();
        string GetDeathFriendReaction(IEnemy enemy);
        string GetStartReaction(int index);
        string GetActionReaction(ActionBattle actionBattle);
        void EndEnemyTurn(int turn);
    }
}