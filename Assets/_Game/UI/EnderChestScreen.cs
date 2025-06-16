using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    public class EnderChestScreen : MonoBehaviour
    {
        [SerializeField]
        private Button _closeButton;

        private GameStateController _gameStateController;

        [Inject]
        private void Construct(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
        }
        
        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveAllListeners();
        }

        private void OnCloseButtonClicked()
        {
            _gameStateController.CloseEnderChest();
        }
    }
}