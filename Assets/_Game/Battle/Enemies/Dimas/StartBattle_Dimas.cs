﻿using I2.Loc;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StartBattle_Dimas : StarterBattle<BattleController_Dimas, BattleController_Dimas.Factory>
    {
        [SerializeField]
        private BattleController_Dimas.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        protected override void Binding(DiContainer subContainer)
        {
            subContainer.BindFactory<BattleController_Dimas, BattleController_Dimas.Factory>()
                .WithArguments(_initData, _localizedStrings);
        }
    }
}