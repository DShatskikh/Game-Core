using DG.Tweening;
using UnityEngine;

namespace Game
{
    public sealed class Shell_Note : Shell
    {
        [SerializeField]
        private float _speed;

        [SerializeField]
        private float _amplitude = 1f;
        
        [SerializeField]
        private float _frequency = 1f;

        [SerializeField]
        private Sprite[] _sprites;
        
        private SpriteRenderer _spriteRenderer;
        private Sequence _sequence;
        private Vector3 _startPosition;

        private float _time;

        private void Awake()
        {
            _startPosition = transform.position;   
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            _spriteRenderer.sprite = _sprites[Random.Range(0, _sprites.Length)];
        }
        
        private void Update()
        {
            if (!IsAlive)
                return;

            _time += Time.deltaTime;

            var newX = transform.position.x - _speed * Time.deltaTime;
            var newY = _startPosition.y + Mathf.Sin(_time * _frequency) * _amplitude;

            transform.position = new Vector3(newX, newY, _startPosition.z);
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