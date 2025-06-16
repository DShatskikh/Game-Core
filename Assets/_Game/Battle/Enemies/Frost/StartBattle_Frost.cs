using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StartBattle_Frost : StarterBattle<BattleController_Frost, BattleController_Frost.Factory>
    {
        [SerializeField]
        private BattleController_Frost.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_Frost, BattleController_Frost.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }
    }
}