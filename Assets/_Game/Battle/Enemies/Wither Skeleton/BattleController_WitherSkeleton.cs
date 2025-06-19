using System;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Unity.Cinemachine;
using Zenject;

namespace Game
{
    public sealed class BattleController_WitherSkeleton : BattleControllerBase
    {
        private protected override string _gameOverMessage => "Все-таки ты не мой бро";
        private readonly InitData _initData;
        private readonly TutorialState _tutorialState;
        private bool _isCreatedTutorialAttack;
        
        [Serializable]
        public struct InitData
        {
            public Enemy_WitherSkeleton Enemy_WitherSkeleton;
            public Attack_WitherSkeleton_Tutorial TutorialAttackPrefab;
        }

        public class Factory : PlaceholderFactory<BattleController_WitherSkeleton> { }

        public BattleController_WitherSkeleton(BattleView view, ShopButton prefabButton, MainInventory inventory, 
            GameStateController gameStateController, BattlePoints points, Player player, Arena arena, Heart heart, 
            DiContainer container, CinemachineCamera virtualCamera, TurnProgressStorage turnProgressStorage,
            TimeBasedTurnBooster timeBasedTurnBooster, EnemyBattleButton enemyBattleButton, ScreenManager screenManager,
            AttackIndicator attackIndicator, INextButton nextButton, 
            SerializableDictionary<string, LocalizedString> localizedPairs, InitData initData, 
            IGameRepositoryStorage mainRepositoryStorage, HealthService healthService, LevelService levelService, 
            WalletService walletService, TutorialState tutorialState, HeartModeService heartModeService, IAssetLoader assetLoader) 
            : base(view, prefabButton, inventory, gameStateController, points, player, 
            arena, heart, container, virtualCamera, turnProgressStorage, timeBasedTurnBooster, enemyBattleButton, 
            screenManager, attackIndicator, nextButton, localizedPairs, mainRepositoryStorage, healthService, 
            levelService, walletService, heartModeService, assetLoader)
        {
            _initData = initData;
            _tutorialState = tutorialState;
            Init();
        }

        private protected override IEnemy[] GetAllEnemies()
        {
            return new IEnemy[]{_initData.Enemy_WitherSkeleton};
        }

        private protected override Attack GetAttack()
        {
            if (!_tutorialState.IsCompleted && !_isCreatedTutorialAttack)
            {
                _isCreatedTutorialAttack = true;
                return _initData.TutorialAttackPrefab;
            }

            if (_enemies[0].Attacks.Length <= _attackIndex)
                _attackIndex = 0;
            
            if (_initData.Enemy_WitherSkeleton.CanMercy)
                return null;
            
            return _enemies[0].Attacks[_attackIndex];
        }

        private protected override string GetStateText()
        {
            if (_initData.Enemy_WitherSkeleton.CanMercy)
                return "Черный скелет щадит вас";
            
            return "Перед вами черный скелет";
        }

        private protected override void EndFightAdditional()
        {
            _tutorialState.FinishStep();
        }
        
        public override void OnGameOver()
        {
            Exit().Forget();
        }
    }
}