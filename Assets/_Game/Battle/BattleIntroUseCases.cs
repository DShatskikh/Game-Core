using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Game
{
    // Распространенные сценарии для битвы
    public static class BattleIntroUseCases
    {
        // Стандартная анимация начала боя
        public static async UniTask WaitIntro(OverWorldPositionsData[] partyPositionsData, OverWorldPositionsData[] enemyPositionsData)
        {
            var moveAnimation = DOTween.Sequence();

            foreach (var overWorldPositionData in partyPositionsData)
                moveAnimation.Insert(0, overWorldPositionData.Transform.DOMove(overWorldPositionData.Point.position, 0.75f));

            foreach (var overWorldPositionData in enemyPositionsData)
                moveAnimation.Insert(0, overWorldPositionData.Transform.DOMove(overWorldPositionData.Point.position, 0.75f));
            
            await moveAnimation.AsyncWaitForCompletion();
        }
    }
}