using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    // Прыгающий по платформам Снинозомби
    public sealed class Shell_Zombie_Jump : Shell
    {
        private Coroutine _coroutine;
        private List<Platform> _platformsDown;
        private Transform _parent;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _coroutine = StartCoroutine(WaitMove());
        }

        public void Init(List<Platform> platformsDown, Transform parent)
        {
            _platformsDown = platformsDown;
            _parent = parent;
        }
        
        private IEnumerator WaitMove()
        {
            yield return new WaitForSeconds(2);

            if (Random.Range(0, 4) == 0)
                yield break;

            var minDistancePlatform = _platformsDown[0];
            var minDistance = float.MaxValue;
            var searchPosition = Random.Range(0, 2) == 0 ? transform.position.AddX(3f) : transform.position.AddX(-1f);

            foreach (var platform in _platformsDown)
            {
                var distance = Vector2.Distance(searchPosition.AddY(-2), platform.transform.position);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minDistancePlatform = platform;
                }
            }

            var target = minDistancePlatform.transform.position.AddX(-1).AddY(0.075f);
            _spriteRenderer.flipX = target.x < transform.position.x;
            yield return transform.DOJump(target, 0.75f, 1, 1).WaitForCompletion();
            transform.SetParent(minDistancePlatform.transform);
            
            yield return new WaitForSeconds(1);
            transform.SetParent(_parent);
            target = transform.position.AddX(Random.Range(-1f, 1f)).AddY(-0.85f);
            _spriteRenderer.flipX = target.x < transform.position.x;
            yield return transform.DOJump(target, 0.75f, 1, 1).WaitForCompletion();
            
            yield return new WaitForSeconds(1);
            target = transform.position.AddX(Random.Range(-1f, 1f)).AddY(-5);
            _spriteRenderer.flipX = target.x < transform.position.x;
            yield return transform.DOJump(target, 1f, 1, 3).WaitForCompletion();
        }

        public override void Hide()
        {
            StopCoroutine(_coroutine);
            IsAlive = false;
            var sequence = DOTween.Sequence();
            sequence.Append(_spriteRenderer.DOColor(Color.clear, 1f));
        }
    }
}