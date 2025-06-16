using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Game
{
    // Логика мини-игры для нанесения атаки
    public sealed class AttackIndicator : MonoBehaviour
    {
        private const float SPEED = 1000f;
        
        [SerializeField]
        private Transform _arrow;

        [SerializeField]
        private AttackPointPair[] _pointPairs;

        [SerializeField]
        private CanvasGroup _canvasGroup;
        
        private int _multiply;
        private INextButton _nextButton;
        private Sequence _sequence;

        [Inject]
        private void Construct(INextButton nextButton)
        {
            _nextButton = nextButton;
        }

        public async UniTask<int> GetMultiply()
        {
            gameObject.SetActive(true);
            _multiply = _pointPairs[0].Multiply;
            _arrow.position = _pointPairs[0].Point.position;
            bool isClicked = false;

            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            transform.localScale = Vector3.one * 0.5f;
            await _sequence
                .Insert(0, transform.DOScale(Vector3.one, 0.5f))
                .Insert(0, DOTween.To(x => _canvasGroup.alpha = x, 0, 1, 0.5f))
                .AppendInterval(0.25f)
                .AsyncWaitForCompletion();
            
            _nextButton.Show(() =>
            {
                isClicked = true;
            });
            
            foreach (var point in _pointPairs)
            {
                while (_arrow.position.x < point.Point.position.x)
                {
                    if (isClicked)
                    {
                        await WaitClose();
                        return _multiply;   
                    }

                    _arrow.position = _arrow.position.AddX(Time.deltaTime * SPEED);
                    await UniTask.WaitForEndOfFrame();
                }

                _multiply = point.Multiply;
            }

            await WaitClose();
            return _multiply;
        }

        private async UniTask WaitClose()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            await _sequence
                .Insert(0, transform.DOScale(Vector3.one * 0.5f, 0.25f))
                .Insert(0, DOTween.To(x => _canvasGroup.alpha = x, 1, 0, 0.25f))
                .AsyncWaitForCompletion();
            
            gameObject.SetActive(false);
        }
    }

    // Структура которая хранит точку на индикаторе и модификатор урона
    [Serializable]
    public struct AttackPointPair
    {
        public Transform Point;
        
        [Range(0, 100)]
        public int Multiply;
    }
}