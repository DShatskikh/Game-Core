using System.Collections.Generic;
using System.Linq;

namespace Game
{
    // Класс персонажа "Банан"
    public sealed class Enemy_Banana : EnemyBase
    {
        private bool _isSubscribe;
        
        public override ActionBattle[] Actions
        {
            get
            {
                var actions = new List<ActionBattle>();

                for (var i = 0; i < _actions.Length; i++)
                {
                    if (i == 1 && _isSubscribe)
                        continue;
                    
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
                    return "Ай";

                case BattleActionType.Mercy:
                    return "Я не хочу обижать своих подписчиков";

                case BattleActionType.NoAction:
                    return "Я стану самым популярным тикитокером";
                
                default:
                    return "(...)";
            }
        }

        public override string GetDeathReaction()
        {
            return "Я думал ты мой фанат";
        }

        public override string GetStartReaction(int index)
        {
            return "Сори, Херобрин попросил одолеть тебя";
        }

        public override string GetActionReaction(ActionBattle actionBattle)
        {
            var action = Actions.First(x => x.Name == actionBattle.Name);

            if (_actions[1].Name == action.Name)
                _isSubscribe = true;
            
            if (_actions[0].Name == action.Name && _isSubscribe)
                return _actions[2].Reaction;
            
            return action.Reaction;
        }
    }
}