using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    // Анимация смерти противника
    public sealed class DeathAnimation : MonoBehaviour
    {
        [SerializeField]
        private SpriteMask _mask;

        [SerializeField]
        private ParticleSystem _particle;
        
        [SerializeField]
        private Transform _pointDown;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        
        [SerializeField]
        private float _duration = 2;

        [Button]
        private void Test()
        {
            StartAnimation();
        }

        public void StartAnimation()
        {
            _spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            _particle.Play();
            
            var sequence = DOTween.Sequence();

            sequence
                .Insert(0, _mask.transform.DOScaleY(0, _duration).SetEase(Ease.OutSine))
                .Insert(0, _mask.transform.DOMoveY(_pointDown.position.y, _duration).SetEase(Ease.OutSine))
                .Insert(0, _particle.transform.DOMoveY(_pointDown.position.y, _duration).SetEase(Ease.OutSine))
                .OnComplete(() =>
                {
                    _particle.Stop();
                    _spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
                    _spriteRenderer.enabled = false;
                });
        }
    }
}