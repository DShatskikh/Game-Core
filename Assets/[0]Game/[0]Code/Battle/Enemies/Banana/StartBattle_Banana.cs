using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    // Класс запускающий битву с персонажем "Банан"
    public sealed class StartBattle_Banana : StarterBattle<BattleController_Banana, BattleController_Banana.Factory>
    {
        [SerializeField]
        private BattleController_Banana.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        [SerializeField]
        private Transform _arrow;
        
        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_Banana, BattleController_Banana.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }

        private protected override void OnStartBattle()
        {
            _arrow.gameObject.SetActive(false);
        }
    }
}