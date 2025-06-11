using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    // Снаряд меч
    public sealed class Shell_Sword_ZombiePigman : Shell
    {
        [SerializeField]
        private float _speed;

        private Sequence _sequence;
        private SpriteRenderer _spriteRenderer;
        private float _direction;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            transform.position += new Vector3(_speed * _direction * Time.deltaTime, 0);
        }

        public override void Hide()
        {
            IsAlive = false;
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(_spriteRenderer.DOColor(Color.clear, 1f));
        }

        public void SetDirection(float direction)
        {
            _direction = direction;

            if (_direction == -1)
            {
                _spriteRenderer.flipX = true;
            }
        }
    }
}