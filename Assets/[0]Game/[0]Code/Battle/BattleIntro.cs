using System.Collections;
using DG.Tweening;

namespace Game
{
    public class BattleIntro : IBattleIntro
    {
        private readonly OverWorldPositionsData[] _partyPositionsData;
        private readonly OverWorldPositionsData[] _enemyPositionsData;
        
        private Sequence _moveAnimation;

        public BattleIntro(OverWorldPositionsData[] partyPositionsData, OverWorldPositionsData[] enemyPositionsData)
        {
            _partyPositionsData = partyPositionsData;
            _enemyPositionsData = enemyPositionsData;
        }
        
        public IEnumerator WaitIntro()
        {
            _moveAnimation?.Kill();
            _moveAnimation = DOTween.Sequence();

            foreach (var overWorldPositionData in _partyPositionsData)
                _moveAnimation.Insert(0, overWorldPositionData.Transform.DOMove(overWorldPositionData.Point.position, 0.75f));

            foreach (var overWorldPositionData in _enemyPositionsData)
                _moveAnimation.Insert(0, overWorldPositionData.Transform.DOMove(overWorldPositionData.Point.position, 0.75f));
            
            yield return _moveAnimation.WaitForCompletion();
        }
    }
}