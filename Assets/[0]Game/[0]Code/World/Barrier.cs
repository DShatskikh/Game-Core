using System;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class Barrier : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private SpriteRenderer[] _spriteRenderers;

        private float[] _startAlphas;
        private Player _player;

        [Inject]
        private void Construct(Player player)
        {
            _player = player;
        }

        private void Start()
        {
            _canvasGroup.alpha = 0;

            _startAlphas = new float[_spriteRenderers.Length];
            
            for (var index = 0; index < _spriteRenderers.Length; index++)
            {
                var spriteRenderer = _spriteRenderers[index];
                _startAlphas[index] = spriteRenderer.color.a;
                spriteRenderer.color = spriteRenderer.color.SetA(0);
            }
        }

        private void Update()
        {
            var isNearly = Vector2.Distance(_player.transform.position, transform.position) < 1;

            if (isNearly)
            {
                for (var index = 0; index < _spriteRenderers.Length; index++)
                {
                    var spriteRenderer = _spriteRenderers[index];
                    var startAlpha = _startAlphas[index];
                    var alpha = spriteRenderer.color.a;
                    
                    if (alpha < startAlpha)
                        spriteRenderer.color = spriteRenderer.color.SetA(alpha + Time.deltaTime * 2);
                }
                
                _canvasGroup.alpha += Time.deltaTime;
            }
            else
            {
                _canvasGroup.alpha -= Time.deltaTime;

                foreach (var spriteRenderer in _spriteRenderers)
                {
                    var alpha = spriteRenderer.color.a;
                    spriteRenderer.color = spriteRenderer.color.SetA(alpha - Time.deltaTime);
                }
            }
        }
    }
}