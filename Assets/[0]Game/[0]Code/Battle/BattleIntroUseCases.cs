using System.Collections;
using DG.Tweening;

namespace Game
{
    public static class BattleIntroUseCases
    {
        public static IEnumerator WaitIntro(OverWorldPositionsData[] partyPositionsData, OverWorldPositionsData[] enemyPositionsData)
        {
            var moveAnimation = DOTween.Sequence();

            foreach (var overWorldPositionData in partyPositionsData)
                moveAnimation.Insert(0, overWorldPositionData.Transform.DOMove(overWorldPositionData.Point.position, 0.75f));

            foreach (var overWorldPositionData in enemyPositionsData)
                moveAnimation.Insert(0, overWorldPositionData.Transform.DOMove(overWorldPositionData.Point.position, 0.75f));
            
            yield return moveAnimation.WaitForCompletion();
        }
    }
}