using System;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Unity.Cinemachine;
using Zenject;

namespace Game
{
    public sealed class BattleController_Dimas : BattleControllerBase
    {
        private readonly InitData _initData;

        [Serializable]
        public struct InitData
        {
            public Enemy_Dimas Enemy_Dimas;
            public PVPArena PvpArena;
        }

        public class Factory : PlaceholderFactory<BattleController_Dimas> { }

        public BattleController_Dimas(BattleView view, ShopButton prefabButton, MainInventory inventory, 
            GameStateController gameStateController, BattlePoints points, Player player, Arena arena, Heart heart, 
            DiContainer container, CinemachineCamera virtualCamera, TurnProgressStorage turnProgressStorage,
            TimeBasedTurnBooster timeBasedTurnBooster, EnemyBattleButton enemyBattleButton, ScreenManager screenManager,
            AttackIndicator attackIndicator, INextButton nextButton, 
            SerializableDictionary<string, LocalizedString> localizedPairs, InitData initData, 
            MainRepositoryStorage mainRepositoryStorage, HealthService healthService, LevelService levelService,
            WalletService walletService) : base(view, prefabButton, inventory, gameStateController, points, player, 
            arena, heart, container, virtualCamera, turnProgressStorage, timeBasedTurnBooster, enemyBattleButton, 
            screenManager, attackIndicator, nextButton, localizedPairs, mainRepositoryStorage, healthService, 
            levelService, walletService)
        {
            _initData = initData;
            Init();
        }

        private protected override IEnemy[] GetAllEnemies()
        {
            return new IEnemy[]{_initData.Enemy_Dimas};
        }

        private protected override Attack GetAttack()
        {
            if (_enemies[0].Attacks.Length <= _attackIndex)
                _attackIndex = 0;
            
            if (_initData.Enemy_Dimas.CanMercy)
                return null;
            
            return _enemies[0].Attacks[_attackIndex];
        }

        private protected override string GetStateText()
        {
            if (_initData.Enemy_Dimas.CanMercy)
                return "Димас проиграл в генста репбатле и щадит вас";
            
            return "В воздухе ветает сила репа";
        }

        public override void OnGameOver()
        {
            Exit().Forget();
        }
        
        private protected override void EndFightAdditional()
        {
            SaveDefeat();
            _initData.PvpArena.StartCoroutine(_initData.PvpArena.AwaitStartCutsceneWinDimas());

            _mainRepositoryStorage.Set(SaveConstants.PVPARENA, 
                new PVPArena.SaveData() { State = PVPArena.State.JULIANA });
        }
    }
}