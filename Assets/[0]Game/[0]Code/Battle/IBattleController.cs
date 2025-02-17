using UniRx;
using UnityEngine;

namespace Game
{
    public interface IBattleController
    {
        IReactiveProperty<float> Progress { get; }
        IBattleController SetEnemies(IEnemy[] enemies);
        IBattleController SetIntro(IBattleIntro intro);
        IBattleController SetOutro(IBattleOutro outro);
        IBattleController SetBattleTheme(AudioClip theme);
        void StartBattle();
        void PlayerTurn();
        IEnemy[] GetEnemies { get; }
        void AddBattleProgress(int progress);
    }
}