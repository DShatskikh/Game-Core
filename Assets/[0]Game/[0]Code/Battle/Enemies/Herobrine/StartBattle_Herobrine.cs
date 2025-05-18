using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StartBattle_Herobrine : StarterBattle<BattleController_Herobrine, BattleController_Herobrine.Factory>
    {
        [SerializeField]
        private BattleController_Herobrine.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_Herobrine, BattleController_Herobrine.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }
    }
}