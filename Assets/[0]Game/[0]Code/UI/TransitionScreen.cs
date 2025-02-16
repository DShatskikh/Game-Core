using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public sealed class TransitionScreen : MonoBehaviour
    {
        private Image _image;
        private Sequence _animation;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void Show(Action action = null)
        {
            gameObject.SetActive(true);
            StartCoroutine(AwaitShow(action));
        }

        public void Hide(Action action = null)
        {
            gameObject.SetActive(true);
            StartCoroutine(AwaitHide(action));
        }

        public IEnumerator AwaitShow(Action action = null)
        {
            _image.color = _image.color.SetA(0);
            _animation = DOTween.Sequence();
            yield return _animation.Append(_image.DOColor(_image.color.SetA(1), 0.5f)).WaitForCompletion();
            gameObject.SetActive(false);
            action?.Invoke();
        }
        
        public IEnumerator AwaitHide(Action action = null)
        {
            _image.color = _image.color.SetA(1);
            _animation = DOTween.Sequence();
            yield return _animation.Append(_image.DOColor(_image.color.SetA(0), 0.5f)).WaitForCompletion();
            gameObject.SetActive(false);
            action?.Invoke();
        }
    }
}