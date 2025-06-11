using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    // Логика движения в красном режиме сердца
    [Serializable]
    public sealed class HeartRedMover : IHeartMover
    {
        [SerializeField]
        private float _speed;

        [SerializeField]
        private Rigidbody2D _rigidbody;
        
        private PlayerInput _playerInput;
        private Transform _transform;

        public void Init(PlayerInput playerInput, Transform transform)
        {
            _transform = transform;
            _playerInput = playerInput;
        }

        void IHeartMover.Enable()
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }

        void IHeartMover.Disable() { }

        void IHeartMover.Move()
        {
            var direction = _playerInput.actions["Move"].ReadValue<Vector2>().normalized;
            Debug.Log(direction);
            _transform.position = (Vector2)_transform.position + direction * _speed * Time.deltaTime;
        }

        void IHeartMover.FixedUpdate() { }
    }
}