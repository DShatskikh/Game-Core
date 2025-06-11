using UnityEngine;

namespace Game
{
    public sealed class TutorialArrow : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private float _indentY = 50;
        
        [SerializeField]
        private float _indentX = 100;
        
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            var _targetPositionScreen = _camera.WorldToScreenPoint(_target.position);
            
            transform.position = transform.position
                .SetX(Mathf.Clamp(_targetPositionScreen.x, _indentX, Screen.width - _indentX))
                .SetY(Mathf.Clamp(_targetPositionScreen.y, _indentY, Screen.height - _indentY));
        }
    }
}