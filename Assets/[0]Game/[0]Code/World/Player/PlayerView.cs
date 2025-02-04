using UnityEngine;

namespace Game
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField]
        private HatView _hatView;

        private SpriteRenderer _spriteRenderer;
        private Animator _animator;

        public bool GetFlipX => _spriteRenderer.flipX;
        public Sprite GetSprite => _spriteRenderer.sprite;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        public void OnDirectionChange(Vector2 direction)
        {
            
        }

        public void OnSpeedChange(float speed)
        {
            
        }
    }
}