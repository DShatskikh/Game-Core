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
    // Обьект которым мы играем во время битвы
    public sealed class Heart : MonoBehaviour
    {
        // Режим души
        public enum Mode : byte
        {
            Red = 0, // Двигаемся во всех направлениях и наш цвет красный
            Blue = 1 // Двигаемся как в платформере и наш цвет синий
        }
        
        private const string ADD_HEALTH_SOUND_PATH = "event:/Звуки/Битва/Восстановили здоровье";
        private const string DAMAGE_SOUND_PATH = "event:/Звуки/Битва/Получили урон";
        private const string DAMAGE_HASH = "Damage";
        private const string NORMAL_HASH = "Normal";
        private const string DEATH_HASH = "Death";
        private const float INVULNERABLE_TIME = 3;
        
        [SerializeField]
        private Shield _shield;
        
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
        
        private bool _isInvulnerability;
        private Arena _arena;
        private Animator _animator;
        private int _damage = 1;
        private IHeartMover _mover;
        private HeartModeService _heartModeService;
        private HealthService _healthService;
        private ArmorService _armorService;

        public event Action OnDeath;

        [Inject]
        private void Construct(Arena arena, PlayerInput playerInput, HeartModeService heartModeService, 
            HealthService healthService, ArmorService armorService)
        {
            _arena = arena;
            _heartModeService = heartModeService;
            _healthService = healthService;
            _armorService = armorService;
            _animator = GetComponent<Animator>();
            
            _heartModeService.Upgrade += SetMode;
            _redMover.Init(playerInput, transform);
            _blueMover.Init(playerInput, _arena);
            SetMode(_heartModeService.GetMode);
        }

        private void OnEnable()
        {
            _shield.gameObject.SetActive(true);
            _isInvulnerability = false;
        }

        private void Update()
        {
            if (_healthService.GetHealth.Value <= 0)
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
                shell.Crash(this);
        }

        private void OnDestroy()
        {
            _heartModeService.Upgrade -= SetMode;
            _mover?.Disable();
        }

        public void SetDamage(int damage) => 
            _damage = damage;

        public void Crash(IShell shell)
        {
            if (!_isInvulnerability && shell.IsAlive)
            {
                _isInvulnerability = true;
                StartCoroutine(TakeDamage(_damage));
            }
        }

        public void SetAddedProgress(int progress) => 
            _shield.SetAddedProgress(progress);

        public void AddHealth(int health)
        {
            RuntimeManager.PlayOneShot(ADD_HEALTH_SOUND_PATH);
            _healthService.Add(health);
        }

        private IEnumerator TakeDamage(int damage)
        {
            RuntimeManager.PlayOneShot(DAMAGE_SOUND_PATH);
            _healthService.GetHealth.Value -= damage * (1 - (int)((float)_armorService.GetArmor / (float)(_armorService.GetArmor + 20f)));

            if (_healthService.GetHealth.Value <= 0)
            {
                Death();   
                yield break;
            }

            _animator.CrossFade(DAMAGE_HASH, 0);
            _shield.gameObject.SetActive(false);
            yield return new WaitForSeconds(INVULNERABLE_TIME);
            _animator.CrossFade(NORMAL_HASH, 0);
            _shield.gameObject.SetActive(true);
            _isInvulnerability = false;
        }

        private void Death()
        {
            _animator.CrossFade(DEATH_HASH, 0);
            OnDeath?.Invoke();
        }

        private void SetMode(Mode mode)
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
                .OnStepComplete(() => _mover.Enable())
                .OnComplete(() => _switchEffectView.gameObject.SetActive(false));
        }
    }
}