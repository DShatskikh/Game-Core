using System;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Unity.Cinemachine;
using Zenject;

namespace Game
{
    // Класс битвы с персонажем "Банан"
    public sealed class BattleController_Banana : BattleControllerBase
    {
        private readonly InitData _initData;

        // Структура с данными битвы для персонажа "Банан"
        [Serializable]
        public struct InitData
        {
            public Enemy_Banana Enemy_Banana;
            public PVPArena PvpArena;
            public WinBananaCutscene WinBananaCutscene;
        }

        public class Factory : PlaceholderFactory<BattleController_Banana> { }

        public BattleController_Banana(BattleView view, ShopButton prefabButton, MainInventory inventory, 
            GameStateController gameStateController, BattlePoints points, Player player, Arena arena, Heart heart, 
            DiContainer container, CinemachineCamera virtualCamera, TurnProgressStorage turnProgressStorage,
            TimeBasedTurnBooster timeBasedTurnBooster, EnemyBattleButton enemyBattleButton, ScreenManager screenManager,
            AttackIndicator attackIndicator, INextButton nextButton, 
            SerializableDictionary<string, LocalizedString> localizedPairs, InitData initData, 
            MainRepositoryStorage mainRepositoryStorage, HealthService healthService, LevelService levelService, 
            WalletService walletService) : base(view, prefabButton, inventory, gameStateController, points, player,
            arena, heart, container, virtualCamera, turnProgressStorage,
            timeBasedTurnBooster, enemyBattleButton, screenManager, attackIndicator, nextButton, localizedPairs,
            mainRepositoryStorage, healthService, levelService, walletService)
        {
            _initData = initData;
            Init();
        }

        private protected override IEnemy[] GetAllEnemies()
        {
            return new IEnemy[]{_initData.Enemy_Banana};
        }

        private protected override Attack GetAttack()
        {
            if (_enemies[0].Attacks.Length <= _attackIndex)
                _attackIndex = 1;
            
            if (_initData.Enemy_Banana.CanMercy)
                return null;
            
            return _enemies[0].Attacks[_attackIndex];
        }

        private protected override string GetStateText()
        {
            if (_initData.Enemy_Banana.CanMercy)
                return "Банан щадит вас";
            
            return "Перед вами стоит Тикитокер";
        }

        public override void OnGameOver()
        {
            Exit().Forget();
        }

        private protected override void EndFightAdditional()
        {
            SaveDefeat();
            _initData.WinBananaCutscene.StartCutscene(true);

            _mainRepositoryStorage.Set(SaveConstants.PVPARENA, 
                new PVPArena.SaveData() { State = PVPArena.State.DIMAS });
        }
    }
}