using UniRx;
using UnityEngine;

namespace Game
{
    public interface IBattleController
    {
        IReactiveProperty<float> Progress { get; }
        IBattleController SetEnemyController(IEnemyController enemyController);
        IBattleController SetIntro(IBattleIntro intro);
        IBattleController SetOutro(IBattleOutro outro);
        IBattleController SetBattleTheme(AudioClip theme);
        void StartBattle();
        void PlayerTurn();
        IEnemyController EnemyController { get; }
        void AddBattleProgress(int progress);
    }
}