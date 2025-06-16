using UnityEngine;

namespace Game
{
    // Движение игрока
    public sealed class PlayerMover : IPlayerMover
    {
        private const float SPEED = 3;
        private const float RUN_SPEED = 7;

        private readonly Rigidbody2D _rigidbody;
        private bool _isMove;

        public bool IsMove => _isMove;
        
        public PlayerMover(Rigidbody2D rigidbody)
        {
            _rigidbody = rigidbody;
        }
        
        public void Move(Vector2 direction, bool isRun)
        {
            _isMove = true;
            _rigidbody.linearVelocity = direction * (isRun ? RUN_SPEED : SPEED);
        }

        public void Stop()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _isMove = false;
        }
    }
}