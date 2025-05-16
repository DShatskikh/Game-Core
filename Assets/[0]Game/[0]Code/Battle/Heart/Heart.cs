using System;
using System.Collections;
using DG.Tweening;
using FMODUnity;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game
{
    public class Heart : MonoBehaviour
    {
        public enum Mode
        {
            Red,
            Blue
        }
        
        private const string DAMAGE_SOUND_PATH = "event:/Звуки/Битва/Получили урон";
        private const string DAMAGE_HASH = "Damage";
        private const string NORMAL_HASH = "Normal";
        private const string DEATH_HASH = "Death";
        
        [SerializeField]
        private Shield _shield;

        [SerializeField] 
        private AudioSource _damageSource;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private SpriteRenderer _switchEffectView;

        [SerializeField]
        private StudioEventEmitter _switchEffectSound;
        
        [SerializeField]
        private HeartRedMover _redMover;

        [SerializeField]
        private HeartBlueMover _blueMover;

        private ReactiveProperty<int> _health = new();
        private int _maxHealth;
        private bool _isInvulnerability;
        private Arena _arena;
        private Animator _animator;
        private int _damage = 1;
        private IHeartMover _mover;
        private HeartModeService _heartModeService;

        public IReactiveProperty<int> GetHealth => _health;
        public event Action OnDeath;
        public int GetMaxHealth => _maxHealth;

        [Inject]
        private void Construct(Arena arena, PlayerInput playerInput, HeartModeService heartModeService)
        {
            _arena = arena;
            _heartModeService = heartModeService;

            heartModeService.Upgrade += SetMode;
            _redMover.Init(playerInput, transform);
            _blueMover.Init(playerInput, _arena);
            
            _animator = GetComponent<Animator>();
            _maxHealth = 20;
            _health.Value = _maxHealth;
            
            SetMode(heartModeService.GetMode);
        }

        private void OnEnable()
        {
            _shield.gameObject.SetActive(true);
            _isInvulnerability = false;
        }

        private void Update()
        {
            if (_health.Value <= 0)
                return;

            _mover.Move();
            var position = (Vector2)transform.position;
            
             if (_arena.IsActive)
             {
                 var limitX = _arena.SizeField.x / 2;
                 var limitY = _arena.SizeField.y / 2;
                 position = new Vector2(
                     Mathf.Clamp(position.x, -limitX + _arena.transform.position.x, limitX + _arena.transform.position.x), 
                     Mathf.Clamp(position.y, -limitY + _arena.transform.position.y, limitY + _arena.transform.position.y));
             }
            
            transform.position = position;
        }

        private void FixedUpdate()
        {
            _mover.FixedUpdate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IShell shell))
            {
                shell.Crash(this);
            }
        }

        public void SetMode(Mode mode)
        {
            _mover?.Disable();
            
            switch (mode)
            {
                case Mode.Red:
                    _mover = _redMover;
                    break;
                case Mode.Blue:
                    _mover = _blueMover;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            var icon = _heartModeService.GetIcon();
            _switchEffectView.gameObject.SetActive(true);
            _switchEffectView.transform.localScale = Vector3.one;
            _switchEffectView.sprite = icon;
            _switchEffectSound.Play();
            _switchEffectView.color = _switchEffectView.color.SetA(0.5f);
            _spriteRenderer.sprite = icon;
            
            var sequence = DOTween.Sequence();
            sequence
                .Insert(0, _switchEffectView.transform.DOScale(Vector3.one * 2, 0.5f))
                .Insert(0, _switchEffectView.DOColor(_switchEffectView.color.SetA(0), 0.5f))
                .OnStepComplete(() =>
                {
                    _mover.Enable();
                })
                .OnComplete(() =>
                {
                    _switchEffectView.gameObject.SetActive(false);
                });
        }
        
        public void SetDamage(int damage)
        {
            _damage = damage;
        }
        
        public void Crash(IShell shell)
        {
            if (!_isInvulnerability && shell.IsAlive)
            {
                _isInvulnerability = true;
                StartCoroutine(TakeDamage(_damage));
            }
        }
        
        private IEnumerator TakeDamage(int damage)
        {
            RuntimeManager.PlayOneShot(DAMAGE_SOUND_PATH);
            _health.Value -= damage;
            _damageSource.Play();

            if (_health.Value <= 0)
            {
                Death();   
                yield break;
            }

            _animator.CrossFade(DAMAGE_HASH, 0);
            _shield.gameObject.SetActive(false);
            yield return new WaitForSeconds(3);
            _animator.CrossFade(NORMAL_HASH, 0);
            _shield.gameObject.SetActive(true);
            _isInvulnerability = false;
        }

        private void Death()
        {
            _animator.CrossFade(DEATH_HASH, 0);
            OnDeath?.Invoke();
        }

        public void SetProgress(int progress)
        {
            _shield.SetProgress(progress);
        }
    }
}