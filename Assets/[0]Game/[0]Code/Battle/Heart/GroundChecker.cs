using UnityEngine;

namespace Game
{
    public sealed class GroundChecker : MonoBehaviour
    {
        [SerializeField]
        private float _rayLength = 1f;
        
        [SerializeField]
        private LayerMask _groundLayer;

        private bool _isGrounded;
        
        public bool GetIsGrounded => _isGrounded;

        private void Update()
        {
            CheckGroundBelow();
        }

        private void CheckGroundBelow()
        {
            Vector2 rayOrigin = transform.position;
            Collider2D col = GetComponent<Collider2D>();

            if (col != null)
            {
                rayOrigin = col.bounds.center;
                rayOrigin.y = col.bounds.min.y;
            }

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, _rayLength, _groundLayer);
            Debug.DrawRay(rayOrigin, Vector2.down * _rayLength, hit.collider != null ? Color.green : Color.red);

            if (hit.collider != null)
            {
                _isGrounded = true;
            }
            else
            {
                _isGrounded = false;
            }
        }
    }
}