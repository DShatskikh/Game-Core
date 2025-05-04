using System.Collections;
using DG.Tweening;
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

        public IEnumerator AwaitSetSize(Vector2 size)
        {
            var startSize = _spriteRenderer.size;
            yield return DOTween
                .To(x => _spriteRenderer.size = Vector2.Lerp(startSize, size, x), 0f, 1f, 1f)
                .WaitForCompletion();
        }
    }
}