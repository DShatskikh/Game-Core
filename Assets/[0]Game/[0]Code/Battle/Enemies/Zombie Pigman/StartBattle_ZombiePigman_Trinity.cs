using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    // Класс запускающий битву с 3 персонажами "Свинозомби"
    public sealed class StartBattle_ZombiePigman_Trinity : StarterBattle<BattleController_ZombiePigman_Trinity, BattleController_ZombiePigman_Trinity.Factory>
    {
        [SerializeField]
        private BattleController_ZombiePigman_Trinity.InitData _initData;
        
        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_ZombiePigman_Trinity, BattleController_ZombiePigman_Trinity.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }
    }
}