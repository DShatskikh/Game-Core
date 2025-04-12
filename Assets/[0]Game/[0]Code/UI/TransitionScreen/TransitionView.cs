using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public sealed class TransitionView : ScreenBase
    {
        [SerializeField]
        private Image _image;
        
        private Sequence _animation;

        public void Toggle(bool value)
        {
            gameObject.SetActive(value);
        }
        
        public IEnumerator PlayShowAnimation()
        {
            _image.color = _image.color.SetA(0);
            _animation = DOTween.Sequence();
            yield return _animation.Append(_image.DOColor(_image.color.SetA(1), 0.5f)).WaitForCompletion();
        }
        
        public IEnumerator PlayHideAnimation()
        {
            _image.color = _image.color.SetA(1);
            _animation = DOTween.Sequence();
            yield return _animation.Append(_image.DOColor(_image.color.SetA(0), 0.5f)).WaitForCompletion();
        }
    }
}