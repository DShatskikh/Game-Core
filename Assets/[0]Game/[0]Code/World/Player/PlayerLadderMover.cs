using UnityEngine;

namespace Game
{
    // Движение игрока по лестнице
    public sealed class PlayerLadderMover : IPlayerMover
    {
        private const float RUN_SPEED = 7;
        private const float WALK_SPEED = 3;
        
        private readonly Transform _transform;
        private readonly Vector2 _startPosition;
        private readonly Vector2 _targetPosition;
        private float _progress;
        private float _distance;

        public PlayerLadderMover(Transform transform, Vector2 startPosition, Vector2 targetPosition, bool isRight)
        {
            _transform = transform;
            _startPosition = startPosition;
            _targetPosition = targetPosition;

            _distance = Vector2.Distance(startPosition, targetPosition);
            
            _progress = isRight ? 1 : 0;

            if (isRight)
                _targetPosition.y = transform.position.y;
            else
                _startPosition.y = transform.position.y;
        }

        public void Move(Vector2 directionValue, bool isRunValue)
        {
            if (directionValue.x == 0)
                return;

            var speed = isRunValue ? RUN_SPEED : WALK_SPEED;

            if (directionValue.x < 0)
            {
                _progress += Time.deltaTime * speed / _distance;

                if (_progress > 1)
                    _progress = 1;
                
                _transform.position = Vector2.Lerp(_startPosition, _targetPosition, _progress);
            }
            else if (directionValue.x > 0)
            {
                _progress -= Time.deltaTime * speed / _distance;
                
                if (_progress < 0)
                    _progress = 0;
                
                _transform.position = Vector2.Lerp(_startPosition, _targetPosition, _progress);
            }
        }

        public void Stop() { }
    }
}