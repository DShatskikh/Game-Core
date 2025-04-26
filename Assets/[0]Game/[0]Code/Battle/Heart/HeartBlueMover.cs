using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    [Serializable]
    public sealed class HeartBlueMover : IHeartMover
    {
        [SerializeField]
        private float _speed = 5f;

        [SerializeField]
        private float _jumpForce = 10f;

        [SerializeField]
        private float _blueModeDuration = 2f;

        [SerializeField]
        private float _blueModeGravityScale = -3f;

        [SerializeField]
        private GroundChecker _groundChecker;

        [SerializeField]
        private Rigidbody2D _rigidbody2D;

        private PlayerInput _playerInput;
        private Transform _transform;
        private float _gravity;
        private float _originalGravity;
        private bool _isBlueMode = false;
        private float _blueModeTimer = 0f;
        private bool _isPressedJump;
        private bool _wasGroundedLastFrame;

        public void Init(PlayerInput playerInput, Transform transform)
        {
            _transform = transform;
            _playerInput = playerInput;
            _originalGravity = Physics2D.gravity.y;
            _playerInput.actions["Jump"].performed += OnJump;
            //_playerInput.actions["BlueMode"].performed += OnBlueMode;
            _wasGroundedLastFrame = _groundChecker.GetIsGrounded;
        }

        public void Move()
        {
            // Обработка таймера синего режима
            if (_isBlueMode)
            {
                _blueModeTimer -= Time.deltaTime;
                if (_blueModeTimer <= 0f)
                {
                    DeactivateBlueMode();
                }
            }

            // Получаем ввод движения
            var direction = _playerInput.actions["Move"].ReadValue<Vector2>().normalized;
            
            // В синем режиме движение по вертикали заменяется на управление гравитацией
            if (_isBlueMode)
            {
                // В синем режиме горизонтальное движение нормальное, а вертикальное влияет на гравитацию
                float horizontal = direction.x;
                float vertical = direction.y;
                
                // Управление гравитацией в синем режиме
                if (Mathf.Abs(vertical) > 0.1f)
                {
                    _rigidbody2D.gravityScale = Mathf.Sign(vertical) * Mathf.Abs(_blueModeGravityScale);
                }
                else
                {
                    // Если нет ввода, гравитация остается как была (но с обратным знаком)
                    _rigidbody2D.gravityScale = _blueModeGravityScale;
                }
                
                // Применяем только горизонтальное движение
                _transform.position += new Vector3(horizontal * _speed * Time.deltaTime, 0, 0);
            }
            else
            {
                // Обычный режим - стандартная платформерная физика
                float horizontal = direction.x;
                
                // Обработка гравитации и прыжка
                if (_groundChecker.GetIsGrounded)
                {
                    _gravity = 0;
                    
                    // Сброс вертикальной скорости при приземлении
                    if (!_wasGroundedLastFrame)
                    {
                        _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, 0);
                    }
                }
                else
                {
                    _gravity += Time.deltaTime * _originalGravity;
                }
                
                _wasGroundedLastFrame = _groundChecker.GetIsGrounded;
                
                // Применяем движение
                Vector2 movement = new Vector2(horizontal * _speed, _rigidbody2D.linearVelocity.y + _gravity * Time.deltaTime);
                _rigidbody2D.linearVelocity = new Vector2(movement.x, Mathf.Clamp(movement.y, -20f, 20f));
            }
        }

        public void FixedUpdate()
        {
            if (_isBlueMode && _groundChecker.GetIsGrounded)
            {
                // Принудительно останавливаем при приземлении в синем режиме
                _rigidbody2D.linearVelocity = Vector2.zero;
                DeactivateBlueMode();
            }
        }

        public void Dispose()
        {
            if (_playerInput != null)
            {
                _playerInput.actions["Jump"].performed -= OnJump;
                //_playerInput.actions["BlueMode"].performed -= OnBlueMode;
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (_groundChecker.GetIsGrounded && !_isBlueMode)
            {
                _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            }
        }

        private void OnBlueMode(InputAction.CallbackContext context)
        {
            // Активируем синий режим только если в воздухе и не в синем режиме
            if (!_groundChecker.GetIsGrounded && !_isBlueMode)
            {
                ActivateBlueMode();
            }
            // Деактивируем синий режим при повторном нажатии
            else if (_isBlueMode)
            {
                DeactivateBlueMode();
            }
        }

        private void ActivateBlueMode()
        {
            _isBlueMode = true;
            _blueModeTimer = _blueModeDuration;
            _rigidbody2D.gravityScale = _blueModeGravityScale;
            
            // При активации синего режима сохраняем текущую горизонтальную скорость
            Vector2 velocity = _rigidbody2D.linearVelocity;
            _rigidbody2D.linearVelocity = new Vector2(velocity.x, 0);
        }

        private void DeactivateBlueMode()
        {
            _isBlueMode = false;
            _blueModeTimer = 0f;
            _rigidbody2D.gravityScale = 1f;
        }
    }
}