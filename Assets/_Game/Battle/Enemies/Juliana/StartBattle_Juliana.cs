using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StartBattle_Juliana : StarterBattle<BattleController_Juliana, BattleController_Juliana.Factory>
    {
        [SerializeField]
        private BattleController_Juliana.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_Juliana, BattleController_Juliana.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }
    }
}