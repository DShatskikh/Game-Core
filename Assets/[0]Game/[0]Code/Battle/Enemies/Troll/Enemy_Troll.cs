using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public sealed class Enemy_Troll : EnemyBase
    {
        public override ActionBattle[] Actions
        {
            get
            {
                var actions = new List<ActionBattle>();

                for (var i = 0; i < _actions.Length; i++)
                {
                    if (i == 2)
                        continue;

                    var action = _actions[i];
                    actions.Add(action);
                }

                return actions.ToArray();
            }
        }
        
        public override string GetReaction(BattleActionType actionType, Item item = null)
        {
            switch (actionType)
            {
                case BattleActionType.Attack:
                    return "Ай. Это не круто";

                case BattleActionType.Mercy:
                    return "Я не хочу обижать своих фанатов";

                case BattleActionType.NoAction:
                    return "Херобрин будет гордиться мной";
                
                default:
                    return "(...)";
            }
        }

        public override string GetDeathReaction()
        {
            return "Я надеялся что превзойду херобрина...";
        }

        public override string GetStartReaction(int index)
        {
            return "Йоу! зацени мой реп";
        }

        public override string GetActionReaction(ActionBattle actionBattle)
        {
            var action = Actions.First(x => x.Name == actionBattle.Name);
            
            if (actionBattle.Name == _actions[1].Name && Mercy >= 75)
                return _actions[2].Reaction;

            return action.Reaction;
        }
    }
}