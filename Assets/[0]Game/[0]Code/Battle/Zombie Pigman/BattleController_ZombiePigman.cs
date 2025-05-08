using System;
using FMODUnity;
using I2.Loc;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class BattleController_ZombiePigman : BattleControllerBase
    {
        private readonly InitData _initData;

        [Serializable]
        public struct InitData
        {
            public Enemy_ZombiePigman Enemy_ZombiePigman;
        }

        public sealed class Factory : PlaceholderFactory<BattleController_ZombiePigman> { }

        public BattleController_ZombiePigman(BattleView view, ShopButton prefabButton, MainInventory inventory, 
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
            if ((_initData.Enemy_ZombiePigman.CanMercy && !_initData.Enemy_ZombiePigman.IsMercy))
                return "Товарищь щадит вас";

            if (_initData.Enemy_ZombiePigman.Health <= 0)
                return "Товарищ хочет отправить вас в Гулаг";
            
            return "Товарищ ждет пока вы покащите документы";
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
            
            if (_enemies[0].Attacks.Length <= _attackIndex)
                _attackIndex = 0;
                
            return _enemies[0].Attacks[_attackIndex];
        }

        private protected override IEnemy[] GetAllEnemies()
        {
            return new IEnemy[]
            {
                _initData.Enemy_ZombiePigman,
            };
        }

        public override void OnGameOver()
        {
            Debug.Log("OnGameOver");
            Exit();
        }
    }
}
     