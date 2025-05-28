using UnityEngine;
using Zenject;

namespace Game
{
    // При открытии и закритии окна диалога переходим в соотвеисивующие стейты
    public sealed class TransitionDialogueState : MonoBehaviour
    {
        private GameStateController _gameStateController;
        private bool _isInit;

        [Inject]
        private void Construct(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
        }

        public void OpenDialogueState()
        {
            if (_isInit)
                _gameStateController.OpenCutscene();
            else
                _isInit = true;
        }
        
        public void CloseDialogueState()
        {
            _gameStateController.CloseCutscene();
        }
    }
}