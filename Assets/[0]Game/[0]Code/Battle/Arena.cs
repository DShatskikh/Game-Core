using UnityEngine;

namespace Game
{
    public class Arena : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        
        public bool IsActive => gameObject.activeSelf;
        public Vector2 SizeField => _spriteRenderer.size - Vector2.one * 0.45f;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}