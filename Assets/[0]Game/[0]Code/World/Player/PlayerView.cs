using UnityEngine;

namespace Game
{
    // Визуальная часть игрока
    public class PlayerView : MonoBehaviour
    {
        private static readonly int StateHash = Animator.StringToHash("State");
        private static readonly int AttackSwordHash = Animator.StringToHash("AttackSword");

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private HatView _hatView;

        [SerializeField]
        private SpriteRenderer _sword;
        
        private Animator _animator;
        private bool _isRun;
        private float _speed;

        public bool GetFlipX => _spriteRenderer.flipX;
        public Sprite GetSprite => _spriteRenderer.sprite;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void OnDirectionChange(Vector2 direction)
        {
            if (direction.x > 0) 
                SetFlipX(false);
                
            if (direction.x < 0) 
                SetFlipX(true);
        }

        public void OnSpeedChange(float speed)
        {
            _speed = speed;
            OnUpgradeSpeed();
        }

        public void OnIsRunChange(bool isRun)
        {
            _isRun = isRun;
            OnUpgradeSpeed();
        }

        public void SetFlipX(bool isFlip)
        {
            _spriteRenderer.flipX = isFlip;
            _hatView.FlipX(isFlip);
        }

        private void OnUpgradeSpeed()
        {
            if (_speed == 0)
                _animator.SetFloat(StateHash, 0);
            else
                _animator.SetFloat(StateHash, _isRun ? 2 : 1);
        }

        public void SwordAttack(Sprite sprite)
        {
            _sword.sprite = sprite;
            _animator.SetTrigger(AttackSwordHash);
        }
    }
}