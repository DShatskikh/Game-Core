using UnityEngine;
using Zenject;

namespace Game
{
    // Закрывает проход для игрока, показывает выбранную надпись
    public sealed class Barrier : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private SpriteRenderer[] _spriteRenderers;

        private float[] _startAlphas;
        private float[] _currentAlphas;
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
            _currentAlphas = new float[_spriteRenderers.Length];
            
            for (var index = 0; index < _spriteRenderers.Length; index++)
            {
                var spriteRenderer = _spriteRenderers[index];
                _startAlphas[index] = spriteRenderer.color.a;
                _currentAlphas[index] = 0;
                spriteRenderer.color = spriteRenderer.color.SetA(0);
            }
        }

        private void Update()
        {
            var isNearly = Vector2.Distance(_player.transform.position, transform.position) < 1;

            if (isNearly)
            {
                for (var index = 0; index < _currentAlphas.Length; index++)
                {
                    var spriteRenderer = _spriteRenderers[index];
                    var startAlpha = _startAlphas[index];
                    
                    if ( _currentAlphas[index] < startAlpha)
                        _currentAlphas[index] += Time.deltaTime * 2;
                    
                    spriteRenderer.color = spriteRenderer.color.SetA( _currentAlphas[index]);
                }
                
                _canvasGroup.alpha += Time.deltaTime;
            }
            else
            {
                _canvasGroup.alpha -= Time.deltaTime;

                for (var index = 0; index < _spriteRenderers.Length; index++)
                {
                    var spriteRenderer = _spriteRenderers[index];
                    _currentAlphas[index] -= Time.deltaTime;

                    if ( _currentAlphas[index] < 0)
                        _currentAlphas[index] = 0;
                    
                    spriteRenderer.color = spriteRenderer.color.SetA( _currentAlphas[index]);
                }
            }
        }
    }
}