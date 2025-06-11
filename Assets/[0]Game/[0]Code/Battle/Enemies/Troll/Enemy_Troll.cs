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
                    return "А мне щекотно!";

                case BattleActionType.Mercy:
                    return "Я не могу с тобой сражаться ты слишком угарный";

                case BattleActionType.NoAction:
                    return "Я тебя затраллирую";
                
                default:
                    return "(...)";
            }
        }

        public override string GetDeathReaction()
        {
            return "Я достиг вершины комедии";
        }

        public override string GetStartReaction(int index)
        {
            return "Я Тролль я тебя затраллирую! Аха-ха-ха!";
        }

        public override string GetActionReaction(ActionBattle actionBattle)
        {
            var action = Actions.First(x => x.Name == actionBattle.Name);
            return action.Reaction;
        }
    }
}