using System;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Unity.Cinemachine;
using Zenject;

namespace Game
{
    // Контроллер битвы Хакера
    public sealed class BattleController_Hacker : BattleControllerBase
    {
        private readonly InitData _initData;

        [Serializable]
        public struct InitData
        {
            public Enemy_Hacker Enemy_Hacker;
            public PVPArena PvpArena;
            public EnemyBattleButton_Hacker EnemyBattleButton;
            public HackerBanCutscene HackerBanCutscene;
        }

        public class Factory : PlaceholderFactory<BattleController_Hacker> { }

        public BattleController_Hacker(BattleView view, ShopButton prefabButton, MainInventory inventory, 
            GameStateController gameStateController, BattlePoints points, Player player, Arena arena, Heart heart, 
            DiContainer container, CinemachineCamera virtualCamera, TurnProgressStorage turnProgressStorage,
            TimeBasedTurnBooster timeBasedTurnBooster, EnemyBattleButton enemyBattleButton, ScreenManager screenManager,
            AttackIndicator attackIndicator, INextButton nextButton, 
            SerializableDictionary<string, LocalizedString> localizedPairs, InitData initData, 
            IGameRepositoryStorage mainRepositoryStorage, HealthService healthService, LevelService levelService, 
            WalletService walletService) : base(view, prefabButton, inventory, 
            gameStateController, points, player, arena, heart, container, virtualCamera, turnProgressStorage,
            timeBasedTurnBooster, enemyBattleButton, screenManager, attackIndicator, nextButton, localizedPairs,
            mainRepositoryStorage, healthService, levelService, walletService)
        {
            _initData = initData;
            SetEnemyPrefabButton(initData.EnemyBattleButton);
            Init();
        }

        private protected override IEnemy[] GetAllEnemies()
        {
            return new IEnemy[]{_initData.Enemy_Hacker};
        }

        private protected override Attack GetAttack()
        {
            if (_enemies[0].Attacks.Length <= _attackIndex)
                _attackIndex = 0;
            
            if (_initData.Enemy_Hacker.CanMercy)
                return null;
            
            return _enemies[0].Attacks[_attackIndex];
        }

        private protected override string GetStateText()
        {
            return "Весь сервер глючит из-за этой битвы";
        }

        public override void OnGameOver()
        {
            Exit().Forget();
        }
        
        private protected override void EndFightAdditional()
        {
            SaveDefeat();
            _initData.PvpArena.StartCoroutine(_initData.PvpArena.AwaitStartCutsceneWinHacker());

            _mainRepositoryStorage.Set(SaveConstants.PVPARENA, 
                new PVPArena.SaveData() { State = PVPArena.State.HEROBRINE });
        }

        private protected override void UpgradeEnemy(EnemyBattleButton enemyButton, IEnemy enemy)
        {
            base.UpgradeEnemy(enemyButton, enemy);

            var health = enemy.Health;
            var button = (EnemyBattleButton_Hacker)enemyButton;
            button.GetHealthLabel.text = $"{health}/{enemy.MaxHealth}";

            if (health < 0)
            {
                button.GetHealthMinusSlider.maxValue = 100;
                button.GetHealthMinusSlider.value = -health;
            }
        }

        public override void Turn()
        {
            if (_numberTurn >= 1)
            {
                CloseAllPanel();
                _initData.HackerBanCutscene.StartCutscene(this);
                return;
            }
            
            base.Turn();
        }
    }
}