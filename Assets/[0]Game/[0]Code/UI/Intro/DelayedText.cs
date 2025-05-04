using System.Collections;
using DG.Tweening;
using Febucci.UI;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace Game
{
    public sealed class DelayedText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _label;
        
        [SerializeField]
        private float _delay;

        [SerializeField]
        private LocalizedString _localizedString;

        private Coroutine _coroutine;
        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _coroutine = StartCoroutine(AwaitAnimation());
        }

        private IEnumerator AwaitAnimation()
        {
            _canvasGroup.alpha = 0;
            _label.text = string.Empty;
            yield return new WaitForSeconds(_delay);
            yield return DOTween.To(x => _canvasGroup.alpha = x, 0, 1, 1).WaitForCompletion();
            _label.text = _localizedString;
        }
        
        public void Skip()
        {
            StopCoroutine(_coroutine);

            _canvasGroup.alpha = 1;
            _label.text = _localizedString;
            StartCoroutine(AwaitSkip());
        }

        private IEnumerator AwaitSkip()
        {
            yield return null;
            _label.GetComponent<TextAnimatorPlayer>().SkipTypewriter();
        }
    }
}