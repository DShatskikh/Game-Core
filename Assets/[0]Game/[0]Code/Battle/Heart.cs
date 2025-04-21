using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game
{
    public class Heart : MonoBehaviour
    {
        [SerializeField]
        private float _speed;
        
        [SerializeField]
        private Shield _shield;

        [SerializeField] 
        private AudioSource _damageSource;
        
        private ReactiveProperty<int> _health = new();
        private int _maxHealth;
        private bool _isInvulnerability;
        private Arena _arena;
        private PlayerInput _playerInput;
        private Animator _animator;
        
        public IReactiveProperty<int> GetHealth => _health;
        public event Action OnDeath;
        public int GetMaxHealth => _maxHealth;

        [Inject]
        private void Construct(Arena arena, PlayerInput playerInput)
        {
            _arena = arena;
            _playerInput = playerInput;

            _animator = GetComponent<Animator>();
            _maxHealth = 20;
            _health.Value = _maxHealth;
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
            
            var position = (Vector2)transform.position;
            
            var direction = _playerInput.actions["Move"].ReadValue<Vector2>().normalized;
            
            position += direction * _speed * Time.deltaTime;

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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IShell shell))
            {
                shell.Crash(this);
            }
        }

        public void Crash()
        {
            if (!_isInvulnerability)
            {
                _isInvulnerability = true;
                StartCoroutine(TakeDamage(5));
            }
        }
        
        private IEnumerator TakeDamage(int damage)
        {
            _health.Value -= damage;
            _damageSource.Play();

            if (_health.Value <= 0)
            {
                Death();   
                yield break;
            }

            _animator.CrossFade("Damage", 0);
            _shield.gameObject.SetActive(false);
            yield return new WaitForSeconds(3);
            _animator.CrossFade("Normal", 0);
            _shield.gameObject.SetActive(true);
            _isInvulnerability = false;
        }

        private void Death()
        {
            _animator.CrossFade("Death", 0);
            OnDeath?.Invoke();
            //EventBus.OnDeath?.Invoke();
            //GameData.GameOver.SetActive(true);

            //Analytics.CustomEvent("Death " + GameData.EnemyData.EnemyConfig.name);
        }
    }
}