﻿using System;
using FMODUnity;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using I2.Loc;

namespace Game
{
    public sealed class BattleController_ZombiePigman_3 : BattleControllerBase
    {
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

        public sealed class Factory : PlaceholderFactory<BattleController_ZombiePigman_3> { }

        public BattleController_ZombiePigman_3(BattleView view, ShopButton prefabButton, MainInventory inventory, 
            GameStateController gameStateController, InitData initData, BattlePoints points, Player player,
            Arena arena, Heart heart, StudioEventEmitter battleThemeEmitter, DiContainer container,
            CinemachineCamera virtualCamera, TurnProgressStorage turnProgressStorage, 
            TimeBasedTurnBooster timeBasedTurnBooster, EnemyBattleButton enemyBattleButton, ScreenManager screenManager, 
            AttackIndicator attackIndicator, INextButton nextButton, 
            SerializableDictionary<string, LocalizedString> localizedPairs) : base(view, prefabButton, inventory, 
            gameStateController, points, player, arena, heart, battleThemeEmitter, container, virtualCamera, 
            turnProgressStorage, timeBasedTurnBooster, enemyBattleButton, screenManager, attackIndicator, nextButton,
            localizedPairs)
        {
            _initData = initData;
            Init();
        }
        
        private protected override string GetStateText()
        {
            if ((_initData.Enemy_ZombiePigman.CanMercy && !_initData.Enemy_ZombiePigman.IsMercy) 
                || (_initData.Enemy_ZombiePigman_1.CanMercy && !_initData.Enemy_ZombiePigman_1.IsMercy) 
                || (_initData.Enemy_ZombiePigman_2.CanMercy && !_initData.Enemy_ZombiePigman_2.IsMercy))
                return "Зомби щадит вас";

            if (_initData.Enemy_ZombiePigman.Health <= 0 || _initData.Enemy_ZombiePigman_1.Health <= 0 || _initData.Enemy_ZombiePigman_2.Health <= 0)
                return "Атмосфера накалилась";
            
            if (_numberTurn == 0)
                return "Зомби ждут вашего хода";
            
            if (_numberTurn == 1)
                return "Зомби просто стоят и тупят";
            
            return "Зомби покорно ждут вашего хода";
        }

        private protected override Attack GetAttack()
        {
            var aliveCount = 0;

            foreach (var enemy in _enemies)
            {
                if (enemy.Health > 0)
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
            Exit();
        }
    }
}
     