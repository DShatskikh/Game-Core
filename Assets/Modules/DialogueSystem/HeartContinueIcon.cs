using DG.Tweening;
using UnityEngine;

namespace Game
{
    // Значек сердце который показывает что реплика напечаталась
    public class HeartContinueIcon : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;
        
        private Sequence _sequence;
        private Vector3 _startPosition;

        private void Awake()
        {
            _startPosition = transform.position;
        }

        private void OnEnable()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            transform.position = _startPosition;
            _sequence.Append(transform.DOMoveY(_target.position.y, 1f).SetEase(Ease.OutSine))
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}