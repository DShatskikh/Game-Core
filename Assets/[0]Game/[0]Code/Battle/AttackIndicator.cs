using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class AttackIndicator : MonoBehaviour
    {
        private const float SPEED = 1000f;
        
        [SerializeField]
        private Transform _arrow;

        [SerializeField]
        private AttackPointPair[] _pointPairs;

        private int _multiply;
        private INextButton _nextButton;

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
            
            _nextButton.Show(() =>
            {
                isClicked = true;
                Debug.Log(_multiply);
            });
            
            foreach (var point in _pointPairs)
            {
                while (_arrow.position.x < point.Point.position.x)
                {
                    if (isClicked)
                    {
                        gameObject.SetActive(false);
                        return _multiply;   
                    }

                    _arrow.position = _arrow.position.AddX(Time.deltaTime * SPEED);
                    await UniTask.WaitForEndOfFrame();
                }

                _multiply = point.Multiply;
            }

            gameObject.SetActive(false);
            return _multiply;
        }
    }

    [Serializable]
    public struct AttackPointPair
    {
        public Transform Point;
        
        [Range(0, 100)]
        public int Multiply;
    }
}