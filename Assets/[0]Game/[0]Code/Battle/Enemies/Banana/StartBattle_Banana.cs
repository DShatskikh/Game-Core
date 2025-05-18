using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StartBattle_Banana : StarterBattle<BattleController_Banana, BattleController_Banana.Factory>
    {
        [SerializeField]
        private BattleController_Banana.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_Banana, BattleController_Banana.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }
    }
}