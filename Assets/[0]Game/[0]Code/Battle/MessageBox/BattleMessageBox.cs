using System.Collections;
using UnityEngine;
using Zenject;

namespace Game
{
    public class BattleMessageBox : MonoBehaviour
    {
        [SerializeField]
        private BattleMessageBoxView _view;

        private Coroutine _coroutine;
        private INextButton _nextButton;

        [Inject]
        private void Construct(INextButton nextButton)
        {
            _nextButton = nextButton;
        }
        
        public IEnumerator AwaitShow(string message)
        {
            _view.ToggleActivate(true);
            _view.SetText(message);

            var i = 5f;
            
            _nextButton.Show(() => i = 0);

            while (i > 0)
            {
                i -= Time.deltaTime;
                yield return null;
            }
            
            _view.ToggleActivate(false);
        }
    }
}