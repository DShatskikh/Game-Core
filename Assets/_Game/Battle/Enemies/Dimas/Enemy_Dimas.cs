using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public sealed class Enemy_Dimas : EnemyBase
    {
        private int _rapCount;
        
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
                    return "Ай. Это не круто";

                case BattleActionType.Mercy:
                    return "Крутой репчик чувак";

                case BattleActionType.NoAction:
                    return "Йоу, ганста, оу щит";
                
                default:
                    return "(...)";
            }
        }

        public override string GetDeathReaction()
        {
            return "Я так и знал что попаду в клуб 21";
        }

        public override string GetStartReaction(int index)
        {
            return "Йоу! зацени мой реп";
        }

        public override string GetActionReaction(ActionBattle actionBattle)
        {
            var action = Actions.First(x => x.Name == actionBattle.Name);

            if (actionBattle.Name == _actions[1].Name)
            {
                if (_rapCount == 1)
                    return "А ты не плох, но про мать было лишнее";
                
                if (_rapCount == 2)
                    return "Прям качает";
                
                if (_rapCount >= 3)
                    return "Чувак мы должны записать лейбл";

                _rapCount++;
            }

            return action.Reaction;
        }
    }
}