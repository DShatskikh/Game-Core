using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    [Serializable]
    public sealed class HeartRedMover : IHeartMover
    {
        [SerializeField]
        private float _speed;

        private PlayerInput _playerInput;
        private Transform _transform;

        public void Init(PlayerInput playerInput, Transform transform)
        {
            _transform = transform;
            _playerInput = playerInput;
        }

        public void Move()
        {
            var direction = _playerInput.actions["Move"].ReadValue<Vector2>().normalized;
            _transform.position = (Vector2)_transform.position + direction * _speed * Time.deltaTime;
        }

        public void FixedUpdate()
        {
            
        }
    }
}