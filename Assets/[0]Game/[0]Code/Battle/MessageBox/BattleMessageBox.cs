using Cysharp.Threading.Tasks;
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

        public static async UniTask AwaitShow(BattleMessageBox[] messageBoxes, string[] messages)
        {
            var _nextButton = messageBoxes[0]._nextButton;
            
            for (int j = 0; j < messageBoxes.Length; j++)
            {
                var messageBox = messageBoxes[j];
                var message = messages[j];
                
                messageBox._view.ToggleActivate(true);
                messageBox._view.SetText(message); 
            }
            
            var i = 5f;
            _nextButton.Show(() => i = 0);

            while (i > 0)
            {
                i -= Time.deltaTime;
                await UniTask.WaitForEndOfFrame();
            }
            
            for (int j = 0; j < messageBoxes.Length; j++)
            {
                var messageBox = messageBoxes[j];
                messageBox._view.ToggleActivate(false);
            }
        }

        public async UniTask AwaitShow(string message)
        {
            _view.ToggleActivate(true);
            _view.SetText(message);

            var i = 5f;
            
            _nextButton.Show(() => i = 0);

            while (i > 0)
            {
                i -= Time.deltaTime;
                await UniTask.WaitForEndOfFrame();
            }
            
            _view.ToggleActivate(false);
        }
    }
}