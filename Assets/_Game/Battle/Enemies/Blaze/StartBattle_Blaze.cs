using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StartBattle_Blaze : StarterBattle<BattleController_Blaze, BattleController_Blaze.Factory>
    {
        [SerializeField]
        private BattleController_Blaze.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_Blaze, BattleController_Blaze.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }
    }
}