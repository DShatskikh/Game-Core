﻿using System;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Unity.Cinemachine;
using UnityEngine.Serialization;
using Zenject;

namespace Game
{
    public sealed class BattleController_Juliana : BattleControllerBase
    {
        private protected override string _gameOverMessage => "Не здавайся!";
        private readonly InitData _initData;

        [Serializable]
        public struct InitData
        {
            public Enemy_Juliana Enemy_Juliana;
            public PVPArena PvpArena;
        }

        public class Factory : PlaceholderFactory<BattleController_Juliana> { }

        public BattleController_Juliana(BattleView view, ShopButton prefabButton, MainInventory inventory, 
            GameStateController gameStateController, BattlePoints points, Player player, Arena arena, Heart heart, 
            DiContainer container, CinemachineCamera virtualCamera, TurnProgressStorage turnProgressStorage,
            TimeBasedTurnBooster timeBasedTurnBooster, EnemyBattleButton enemyBattleButton, ScreenManager screenManager,
            AttackIndicator attackIndicator, INextButton nextButton, 
            SerializableDictionary<string, LocalizedString> localizedPairs, InitData initData, 
            IGameRepositoryStorage mainRepositoryStorage, HealthService healthService, LevelService levelService, 
            WalletService walletService, HeartModeService heartModeService, IAssetLoader assetLoader) : base(view, prefabButton, inventory, 
            gameStateController, points, player, arena, heart, container, virtualCamera, turnProgressStorage,
            timeBasedTurnBooster, enemyBattleButton, screenManager, attackIndicator, nextButton, localizedPairs, 
            mainRepositoryStorage, healthService, levelService, walletService, heartModeService, assetLoader)
        {
            _initData = initData;
            Init();
        }

        private protected override IEnemy[] GetAllEnemies()
        {
            return new IEnemy[]{_initData.Enemy_Juliana};
        }

        private protected override Attack GetAttack()
        {
            if (_enemies[0].Attacks.Length <= _attackIndex)
                _attackIndex = 0;
            
            if (_initData.Enemy_Juliana.CanMercy)
                return null;
            
            return _enemies[0].Attacks[_attackIndex];
        }

        private protected override string GetStateText()
        {
            if (_initData.Enemy_Juliana.CanMercy)
                return "Банан щадит вас";
            
            return "Сильнейший телохранитель Херобрина (по его словам)";
        }

        public override void OnGameOver()
        {
            Exit().Forget();
        }
        
        private protected override void EndFightAdditional()
        {
            SaveDefeat();
            _initData.PvpArena.StartCoroutine(_initData.PvpArena.AwaitStartCutsceneWinJuliana());

            _mainRepositoryStorage.Set(SaveConstants.PVPARENA, 
                new PVPArena.SaveData() { State = PVPArena.State.TROLL });
        }
    }
}