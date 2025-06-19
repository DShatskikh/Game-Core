using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public sealed class Enemy_WitherSkeleton : EnemyBase
    {
        private int _cockCount;
        
        public override ActionBattle[] Actions
        {
            get
            {
                var actions = new List<ActionBattle>();

                for (var i = 0; i < _actions.Length; i++)
                {
                    var action = _actions[i];
                    
                    if (i == 0 && CanMercy)
                        continue;
                    
                    if (i == 1 && !CanMercy)
                        continue;
                    
                    if (i == 2 && _cockCount == 4)
                        continue;
                    
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
                    return "Быть скелетом так тяжело!";

                case BattleActionType.Mercy:
                    return "Я ты мой бро";

                case BattleActionType.NoAction:
                    return "Ты не мой бро";
                
                default:
                    return "(...)";
            }
        }

        public override string GetDeathReaction()
        {
            return "Я думал ты мой бро";
        }

        public override string GetStartReaction(int index)
        {
            return "Ты не мой бро";
        }

        public override string GetActionReaction(ActionBattle actionBattle)
        {
            var action = Actions.First(x => x.Name == actionBattle.Name);
            
            if (actionBattle.Name == _actions[2].Name) 
                _cockCount++;

            return action.Reaction;
        }
    }
}