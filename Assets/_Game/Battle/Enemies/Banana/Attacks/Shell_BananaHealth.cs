using DG.Tweening;
using UnityEngine;

namespace Game
{
    public sealed class Shell_BananaHealth : MonoBehaviour, IShell
    {
        [SerializeField]
        private int _health;
        
        [SerializeField]
        private float _speed;

        private SpriteRenderer _spriteRenderer;
        private Sequence _sequence;

        public bool IsDestroy => true;
        public bool IsAlive { get; set; } = true;
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
        
        public void Crash(Heart heart)
        {
            heart.AddHealth(_health);
            Destroy(gameObject);
        }

        public void Hide()
        {
            IsAlive = false;
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(_spriteRenderer.DOColor(Color.clear, 1f));
        }
    }
}