using System;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Unity.Cinemachine;
using Zenject;

namespace Game
{
    // Класс битвы с персонажем "Свинозомби"
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
            Arena arena, Heart heart, DiContainer container,
            CinemachineCamera virtualCamera, TurnProgressStorage turnProgressStorage, 
            TimeBasedTurnBooster timeBasedTurnBooster, EnemyBattleButton enemyBattleButton, ScreenManager screenManager, 
            AttackIndicator attackIndicator, INextButton nextButton, 
            SerializableDictionary<string, LocalizedString> localizedPairs,
            MainRepositoryStorage mainRepositoryStorage, HealthService healthService, LevelService levelService) : base(view, prefabButton, inventory, 
            gameStateController, points, player, arena, heart, container, virtualCamera, 
            turnProgressStorage, timeBasedTurnBooster, enemyBattleButton, screenManager, attackIndicator, nextButton,
            localizedPairs, mainRepositoryStorage, healthService, levelService)
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
            if (_initData.Enemy_ZombiePigman.CanMercy && !_initData.Enemy_ZombiePigman.IsMercy)
            {
                if (_initData.Enemy_ZombiePigman.IsBuySword)
                    return "Свинозомби нечем драться он щадит вас";
                
                return "Свинозомби расхотел с вами драться он щадит вас";
            }

            return "Свинозомби хочет вас побить";
        }

        private protected override Attack GetAttack()
        {
            if (_enemies[0].Attacks.Length <= _attackIndex)
                _attackIndex = 0;
                
            if (_initData.Enemy_ZombiePigman.IsBuySword)
                return null;
            
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
            Exit().Forget();
        }
    }
}
     