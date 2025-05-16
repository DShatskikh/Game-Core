using System;
using UnityEngine;

namespace Game
{
    public sealed class Platform : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _direction = Vector2.right;
        
        [SerializeField]
        private float _speed = 1;

        private bool _isMove = true;
        private Heart _heart;
        
        private void Update()
        {
            if (!_isMove)
                return;
            
            transform.position += (Vector3)_direction * _speed * Time.deltaTime;

            if (_heart)
            {
                _heart.transform.position += (Vector3)_direction * _speed * Time.deltaTime;
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            _heart = null;
            
            if (other.collider.TryGetComponent(out Heart heart))
            {
                _heart = heart;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.TryGetComponent(out Heart heart))
            {
                _heart = null;
            }
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        public void Hide()
        {
            _isMove = false;
        }
    }
}