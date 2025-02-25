using UnityEngine.UI;
using Zenject;

namespace Game
{
    public sealed class DialogueButton : Button
    {
        private INextButton _nextButton;

        [Inject]
        private void Construct(INextButton nextButton)
        {
            _nextButton = nextButton;
        }
        
        protected override void OnEnable()
        {
            if (_nextButton == null)
                return;
            
            base.OnEnable();
            _nextButton.Show(() =>
            {
                onClick.Invoke();
            });
        }
    }
}