﻿using System;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using I2.Loc;

namespace Game
{
    // Класс битвы с 3 персонажами "Свинозомби"
    public sealed class BattleController_ZombiePigman_Trinity : BattleControllerBase
    {
        private protected override string _gameOverMessage => "Ты умер от Свино Зомбиии!";
        private readonly InitData _initData;

        [Serializable]
        public struct InitData
        {
            public Enemy_ZombiePigman Enemy_ZombiePigman;
            public Enemy_ZombiePigman Enemy_ZombiePigman_1;
            public Enemy_ZombiePigman Enemy_ZombiePigman_2;
            
            public Attack[] TwoZombiePigmanAttacks;
            public Attack[] ThreeZombiePigmanAttacks;
        }

        public sealed class Factory : PlaceholderFactory<BattleController_ZombiePigman_Trinity> { }

        public BattleController_ZombiePigman_Trinity(BattleView view, ShopButton prefabButton, MainInventory inventory, 
            GameStateController gameStateController, InitData initData, BattlePoints points, Player player,
            Arena arena, Heart heart, DiContainer container,
            CinemachineCamera virtualCamera, TurnProgressStorage turnProgressStorage, 
            TimeBasedTurnBooster timeBasedTurnBooster, EnemyBattleButton enemyBattleButton, ScreenManager screenManager, 
            AttackIndicator attackIndicator, INextButton nextButton, 
            SerializableDictionary<string, LocalizedString> localizedPairs,
            IGameRepositoryStorage mainRepositoryStorage, HealthService healthService, LevelService levelService, 
            WalletService walletService, HeartModeService heartModeService, IAssetLoader assetLoader) : base(view, prefabButton, inventory, gameStateController, points, player,
            arena, heart, container, virtualCamera, turnProgressStorage, timeBasedTurnBooster, enemyBattleButton,
            screenManager, attackIndicator, nextButton, localizedPairs, mainRepositoryStorage, healthService, 
            levelService, walletService, heartModeService, assetLoader)
        {
            _initData = initData;
            Init();
        }
        
        protected override async UniTask StartBattle()
        {
            CloseAllPanel();
            await ShowEnemiesReactions(GetStartReactions());
            EnemyTurn().Forget();
        }
        
        private protected override string GetStateText()
        {
            if ((_initData.Enemy_ZombiePigman.CanMercy && !_initData.Enemy_ZombiePigman.IsMercy) 
                || (_initData.Enemy_ZombiePigman_1.CanMercy && !_initData.Enemy_ZombiePigman_1.IsMercy) 
                || (_initData.Enemy_ZombiePigman_2.CanMercy && !_initData.Enemy_ZombiePigman_2.IsMercy))
                return "Свинозомби щадит вас";

            if (_initData.Enemy_ZombiePigman.IsDeath || _initData.Enemy_ZombiePigman_1.IsDeath || _initData.Enemy_ZombiePigman_2.IsDeath)
                return "Атмосфера накалилась";
            
            return "Свинозомби хотят вас побить";
        }

        private protected override Attack GetAttack()
        {
            var aliveCount = 0;

            foreach (var enemy in _enemies)
            {
                if (!enemy.IsDeath)
                    aliveCount++;
            }

            Debug.Log($"{aliveCount} {_enemies[0].Attacks.Length}");
            
            if (aliveCount == 1)
            {
                if (_enemies[0].Attacks.Length <= _attackIndex)
                    _attackIndex = 0;
                
                return _enemies[0].Attacks[_attackIndex];
            }

            if (aliveCount == 2)
            {
                if (_initData.TwoZombiePigmanAttacks.Length <= _attackIndex)
                    _attackIndex = 0;
                
                return _initData.TwoZombiePigmanAttacks[_attackIndex];
            }

            if (_initData.ThreeZombiePigmanAttacks.Length <= _attackIndex)
                _attackIndex = 0;
            
            return _initData.ThreeZombiePigmanAttacks[_attackIndex];
        }

        private protected override IEnemy[] GetAllEnemies()
        {
            return new IEnemy[]
            {
                _initData.Enemy_ZombiePigman,
                _initData.Enemy_ZombiePigman_1,
                _initData.Enemy_ZombiePigman_2
            };
        }

        public override void OnGameOver()
        {
            Debug.Log("OnGameOver");
            Exit().Forget();
        }
    }
}
     