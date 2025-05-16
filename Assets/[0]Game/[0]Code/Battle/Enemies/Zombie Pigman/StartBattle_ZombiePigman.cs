using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StartBattle_ZombiePigman : StarterBattle<BattleController_ZombiePigman, BattleController_ZombiePigman.Factory>
    {
        [SerializeField]
        private BattleController_ZombiePigman.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_ZombiePigman, BattleController_ZombiePigman.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }
    }
}