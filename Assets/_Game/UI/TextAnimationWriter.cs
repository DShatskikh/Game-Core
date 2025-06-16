using System;
using Febucci.UI;
using UnityEngine;
using Zenject;

namespace Game
{
    // Текст который можно пропустить тапом
    public class TextAnimationWriter : MonoBehaviour
    {
        [SerializeField]
        private TextAnimatorPlayer _textAnimatorPlayer;
        
        private INextButton _nextButton;

        [Inject]
        private void Construct(INextButton nextButton)
        {
            _nextButton = nextButton;
        }
        
        public void StartWrite(string[] text, Action textShowed, int index = 0)
        {
            _textAnimatorPlayer.ShowText(text[index]);
            gameObject.SetActive(true);

            if (index != text.Length - 1)
            {
                _nextButton.Show(() => StartWrite(text, textShowed, index + 1));
            }
            else
            {
                _nextButton.Show(() => textShowed?.Invoke());
            }
        }
    }
}