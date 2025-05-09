using UnityEngine;

namespace Game
{
    public sealed class Parallax : MonoBehaviour
    {
        private enum ParallaxDirectionType : byte
        {
            All,
            Horizontal,
            Vertical,
        }

        [SerializeField]
        private float _multiplyDistance = 1f;

        [SerializeField]
        private ParallaxDirectionType _parallaxDirectionType;
        
        private Transform _camera;
        private Vector3 _startPosition;

        private void Awake()
        {
            _startPosition = transform.position;
            _camera = Camera.main.transform;
        }

        private void Update()
        {
            var direction = (_startPosition - _camera.position) * _multiplyDistance;
            
            if (_parallaxDirectionType == ParallaxDirectionType.Horizontal ||
                _parallaxDirectionType == ParallaxDirectionType.All)
            {
                transform.position = transform.position.SetX(direction.x);
            }

            if (_parallaxDirectionType == ParallaxDirectionType.Vertical ||
                _parallaxDirectionType == ParallaxDirectionType.All)
            {
                transform.position = transform.position.SetY(direction.y);
            }
        }
    }
}