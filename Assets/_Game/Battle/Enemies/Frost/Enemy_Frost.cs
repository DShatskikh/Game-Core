using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public sealed class Enemy_Frost : EnemyBase
    {
        private int _potatoCount = 3;
        
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

        private protected override void OnDamage()
        {
            _potatoCount--;
            // Ест картошку, здается на 5 ходе потому что видит что мы достойны чтобы попасть в спарту
        }

        public override string GetReaction(BattleActionType actionType, Item item = null)
        {
            switch (actionType)
            {
                case BattleActionType.Attack:
                    return "Нужно поесть картошки";

                case BattleActionType.Mercy:
                    return "Да прибудет с тобой Спарта";

                case BattleActionType.NoAction:
                    return "Это СПАААРТА!";
                
                default:
                    return "(...)";
            }
        }

        public override string GetDeathReaction()
        {
            return "Черт, я програл. Я иду к тебе Леонид";
        }

        public override string GetStartReaction(int index)
        {
            return "Это Спарта!";
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