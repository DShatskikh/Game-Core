using System.Collections;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Shell_Sworld_Herobrine : Shell
    {
        private SpriteRenderer _spriteRenderer;
        private Heart _heart;
        private Coroutine _coroutine;
        private Sequence _sequence;

        [Inject]
        private void Construct(Heart heart)
        {
            _heart = heart;

            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        private void Start()
        {
            _coroutine = StartCoroutine(WaitMove());
        }

        private IEnumerator WaitMove()
        {
            var targetRotate = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,
                Mathf.Atan2(_heart.transform.position.y - transform.position.y,
                    _heart.transform.position.x - transform.position.x) * Mathf.Rad2Deg);
            
            transform.eulerAngles = targetRotate.eulerAngles.AddZ(90);

            while (transform.rotation != targetRotate)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotate, 400 * Time.deltaTime);
                yield return null;
            }

            var timer = 0.5f;
            
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                transform.position -= transform.right * Time.deltaTime;
                yield return null;
            }
            
            while (true)
            {
                transform.position += transform.right * 6 * Time.deltaTime;
                yield return null;
            }
        }
        
        public override void Hide()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            IsAlive = false;
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(_spriteRenderer.DOColor(Color.clear, 1f));
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
    }
}