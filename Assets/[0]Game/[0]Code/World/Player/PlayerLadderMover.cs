using UnityEngine;

namespace Game
{
    public sealed class PlayerLadderMover : IPlayerMover
    {
        private readonly Transform _transform;
        private readonly Vector2 _startPosition;
        private readonly Vector2 _targetPosition;
        private float _progress;
        private readonly bool _isRight;
        public bool IsMove { get; }
        public bool IsRun => _isRight;

        public PlayerLadderMover(Transform transform, Vector2 startPosition, Vector2 targetPosition, bool isRight)
        {
            _transform = transform;
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            _isRight = isRight;

            _progress = _isRight ? 1 : 0;

            if (_isRight)
                _targetPosition.y = transform.position.y;
            else
                _startPosition.y = transform.position.y;
        }

        public void Move(Vector2 directionValue, bool isRunValue)
        {
            var speed = isRunValue ? 2 : 1;
            
            if (directionValue.x < 0)
            {
                _progress += Time.deltaTime * speed;

                if (_progress > 1)
                    _progress = 1;
                
                _transform.position = Vector2.Lerp(_startPosition, _targetPosition, _progress);
            }
            else if (directionValue.x > 0)
            {
                _progress -= Time.deltaTime * speed;
                
                if (_progress < 0)
                    _progress = 0;
                
                _transform.position = Vector2.Lerp(_startPosition, _targetPosition, _progress);
            }
        }

        public void Stop()
        {
            
        }
    }
}