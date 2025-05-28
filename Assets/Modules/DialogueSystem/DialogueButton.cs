using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    // Кнопка в окне диалога
    public sealed class DialogueButton : Button
    {
        [SerializeField]
        private TextAnimatorTypewriterEffect _animatorTypewriter;

        private INextButton _nextButton;
        private bool _isShow;

        [Inject]
        private void Construct(INextButton nextButton)
        {
            _nextButton = nextButton;
        }
        
        protected override void OnEnable()
        {
            if (_nextButton == null)
                return;
            
            _isShow = true;
            base.OnEnable();
            _nextButton.Show(Click);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _isShow = false;
        }

        private void Click()
        {
            if (_animatorTypewriter.isPlaying)
            {
                _animatorTypewriter.StopTyping();
            }
            else
            {
                onClick.Invoke();
            }
            
            if (_isShow)
                _nextButton.Show(Click);
        }
    }
}