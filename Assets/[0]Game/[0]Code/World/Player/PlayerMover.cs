using UnityEngine;

namespace Game
{
    // Движение игрока
    public sealed class PlayerMover : IPlayerMover
    {
        private const float SPEED = 3;
        private const float RUN_SPEED = 7;

        private bool _isMove;
        private bool _isRun;
        
        private readonly Rigidbody2D _rigidbody;

        public bool IsMove => _isMove;
        public bool IsRun => _isRun;

        public PlayerMover(Rigidbody2D rigidbody)
        {
            _rigidbody = rigidbody;
        }
        
        public void Move(Vector2 direction, bool isRun)
        {
            _isRun = isRun;
            _rigidbody.linearVelocity = direction * (isRun ? RUN_SPEED : SPEED);
            _isMove = true;
        }

        public void Stop()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _isMove = false;
        }
    }
}