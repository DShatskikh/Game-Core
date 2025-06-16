using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    // Инсталлер Хакер
    public sealed class StartBattle_Hacker : StarterBattle<BattleController_Hacker, BattleController_Hacker.Factory>
    {
        [SerializeField]
        private BattleController_Hacker.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_Hacker, BattleController_Hacker.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }
    }
}