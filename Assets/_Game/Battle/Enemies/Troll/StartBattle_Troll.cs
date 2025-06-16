using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StartBattle_Troll : StarterBattle<BattleController_Troll, BattleController_Troll.Factory>
    {
        [SerializeField]
        private BattleController_Troll.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_Troll, BattleController_Troll.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }
    }
}