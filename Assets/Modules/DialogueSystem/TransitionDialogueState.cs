using UnityEngine;
using Zenject;

namespace Game
{
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
                _gameStateController.OpenDialog();
            else
                _isInit = true;
        }
        
        public void CloseDialogueState()
        {
            _gameStateController.CloseDialog();
        }
    }
}