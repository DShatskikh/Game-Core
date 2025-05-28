using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    // Логика движения в синем режиме сердца
    [Serializable]
    public sealed class HeartBlueMover : IHeartMover
    {
        [Header("Movement Settings")]
        [SerializeField]
        private float _moveAcceleration = 10f;

        [SerializeField]
        private float _maxHorizontalSpeed = 2f;

        [SerializeField]
        private float _jumpPower = 3f;

        [SerializeField]
        private float _jumpAdditional = 5f;

        [SerializeField]
        private float _friction = 0.2f;

        [SerializeField]
        private float _gravity = -10f;

        [SerializeField]
        private float _maxFallSpeed = -120f;

        [SerializeField]
        private LayerMask _floorLayer;

        [SerializeField]
        private CeilingChecker _ceilingChecker;

        [SerializeField]
        private Rigidbody2D _rigidbody2D;
        
        [SerializeField]
        private Collider2D _collider;

        private float _horizontalSpeed;
        private float _verticalSpeed;
        private bool _isGrounded = false;
        private PlayerInput _playerInput;
        private Arena _arena;

        public void Init(PlayerInput playerInput, Arena arena)
        {
            _playerInput = playerInput;
            _arena = arena;
        }

        void IHeartMover.Enable()
        {
            _playerInput.actions["Jump"].started += OnClickJumpStarted;
        }

        void IHeartMover.Disable()
        {
            _playerInput.actions["Jump"].started -= OnClickJumpStarted;

            _isGrounded = false;
        }

        void IHeartMover.Move()
        {
            HandleInput();
            CheckGrounded();
        }

        void IHeartMover.FixedUpdate()
        {
            ApplyMovement();
            ApplyGravity();
            ApplyFriction();
            ClampSpeed();
            CheckCeiling();
        }

        private void OnClickJumpStarted(InputAction.CallbackContext obj)
        {
            if (_isGrounded)
            {
                _verticalSpeed = _jumpPower;
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _verticalSpeed);
                _isGrounded = false;
            }
        }

        private void HandleInput()
        {
            var moveInput =_playerInput.actions["Move"].ReadValue<Vector2>().normalized.x;
            _horizontalSpeed = moveInput * _moveAcceleration;

            if (_playerInput.actions["Jump"].IsPressed())
            {
                if (!_isGrounded)
                {
                    _verticalSpeed += _jumpAdditional * Time.deltaTime;
                }
            }
        }

        private void ApplyMovement()
        {
            _rigidbody2D.linearVelocity = new Vector2(_horizontalSpeed, _rigidbody2D.linearVelocity.y);
        }

        private void ApplyGravity()
        {
            if (!_isGrounded)
            {
                _verticalSpeed += _gravity * Time.fixedDeltaTime;
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _verticalSpeed);
            }
            else if (_verticalSpeed < 0)
            {
                _verticalSpeed = 0;
            }
        }

        private void ApplyFriction()
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f)
            {
                _horizontalSpeed *= (1 - _friction);
                if (Mathf.Abs(_horizontalSpeed) < 0.1f)
                    _horizontalSpeed = 0f;
            }
        }

        private void ClampSpeed()
        {
            _rigidbody2D.linearVelocity = new Vector2(
                Mathf.Clamp(_rigidbody2D.linearVelocity.x, -_maxHorizontalSpeed, _maxHorizontalSpeed),
                _rigidbody2D.linearVelocity.y
            );

            if (_rigidbody2D.linearVelocity.y < _maxFallSpeed)
            {
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _maxFallSpeed);
            }
        }

        private void CheckGrounded()
        {
            var rayLength = 0.1f;
            var hit = Physics2D.Raycast(_collider.bounds.center, Vector2.down, _collider.bounds.extents.y + rayLength,
                _floorLayer);
            
            _isGrounded = hit.collider != null || _collider.transform.position.y <= _arena.transform.position.y - _arena.SizeField.y / 2 + 0.01f;
            Debug.DrawRay(_collider.bounds.center, Vector2.down * (_collider.bounds.extents.y + rayLength), Color.red);
        }

        private void CheckCeiling()
        {
            if (_ceilingChecker.GetIsTouchingCeiling)
            {
                _verticalSpeed = _gravity * Time.fixedDeltaTime;
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _verticalSpeed);
            }
        }
    }
}