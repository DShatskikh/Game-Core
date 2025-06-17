using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public sealed class Enemy_WitherSkeleton : EnemyBase
    {
        private int _speak;
        
        public override ActionBattle[] Actions
        {
            get
            {
                var actions = new List<ActionBattle>();

                for (var i = 0; i < _actions.Length; i++)
                {
                    var action = _actions[i];
                    
                    switch (i)
                    {
                        case 1 when _speak != 0:
                        case 3 when _speak != 1:
                        case 4 when _speak < 2:
                            continue;
                        default:
                            actions.Add(action);
                            break;
                    }
                }

                return actions.ToArray();
            }
        }
        
        public override string GetReaction(BattleActionType actionType, Item item = null)
        {
            switch (actionType)
            {
                case BattleActionType.Attack:
                    return "Девочек бить нельзя!";

                case BattleActionType.Mercy:
                    return "Я больше не в команде Херобрина";

                case BattleActionType.NoAction:
                    return "Я просто хочу домой";
                
                default:
                    return "(...)";
            }
        }

        public override string GetDeathReaction()
        {
            return "Я думала ты хороший";
        }

        public override string GetStartReaction(int index)
        {
            return "Сори, но я в команде Херобрина, поэтому я сражаюсь против тебя";
        }

        public override string GetActionReaction(ActionBattle actionBattle)
        {
            var action = Actions.First(x => x.Name == actionBattle.Name);
            
            if (actionBattle.Name == _actions[1].Name || actionBattle.Name == _actions[3].Name || actionBattle.Name == _actions[4].Name)
            {
                _speak++;
            }

            return action.Reaction;
        }
    }
}