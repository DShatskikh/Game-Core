using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    // Класс запускающий битву с 3 персонажами "Свинозомби"
    public sealed class StartBattle_ZombiePigman_3 : StarterBattle<BattleController_ZombiePigman_3, BattleController_ZombiePigman_3.Factory>
    {
        [SerializeField]
        private BattleController_ZombiePigman_3.InitData _initData;
        
        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_ZombiePigman_3, BattleController_ZombiePigman_3.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }
    }
}