using System;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Unity.Cinemachine;
using UnityEngine.Serialization;
using Zenject;

namespace Game
{
    public sealed class BattleController_Herobrine : BattleControllerBase
    {
        private protected override string _gameOverMessage => "Ха-ха нубяра";
        private readonly InitData _initData;

        [Serializable]
        public struct InitData
        {
            public Enemy_Herobrine Enemy_Herobrine;
            public PVPArena PvpArena;
        }

        public class Factory : PlaceholderFactory<BattleController_Herobrine> { }

        public BattleController_Herobrine(BattleView view, ShopButton prefabButton, MainInventory inventory, 
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
            return new IEnemy[]{_initData.Enemy_Herobrine};
        }

        private protected override Attack GetAttack()
        {
            if (_enemies[0].Attacks.Length <= _attackIndex)
                _attackIndex = 0;
            
            if (_initData.Enemy_Herobrine.CanMercy)
                return null;
            
            return _enemies[0].Attacks[_attackIndex];
        }

        private protected override string GetStateText()
        {
            if (_initData.Enemy_Herobrine.CanMercy)
                return "Херобрин устал сражаться и поэтому щадит вас";
            
            return "Вот и финал";
        }

        public override void OnGameOver()
        {
            Exit().Forget();
        }
        
        private protected override void EndFightAdditional()
        {
            SaveDefeat();
            _initData.PvpArena.StartCoroutine(_initData.PvpArena.AwaitStartCutsceneWinHerobrine());
            
            _mainRepositoryStorage.Set(SaveConstants.PVPARENA, 
                new PVPArena.SaveData() { State = PVPArena.State.END });
        }
    }
}