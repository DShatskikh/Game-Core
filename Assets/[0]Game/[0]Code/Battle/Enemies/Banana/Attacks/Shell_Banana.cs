using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class Shell_Banana : Shell
    {
        [SerializeField]
        private float _speed;

        private SpriteRenderer _spriteRenderer;
        private Sequence _sequence;
        
        public Vector2 Direction;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void Update()
        {
            transform.position += _speed * (Vector3)Direction * Time.deltaTime;
        }
        
        public void SetDirection(Vector2 direction)
        {
            Direction = direction;
        }

        public override void Hide()
        {
            IsAlive = false;
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(_spriteRenderer.DOColor(Color.clear, 1f));
        }
    }
}