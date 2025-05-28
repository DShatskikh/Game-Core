using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game
{
    // Меню игры
    public sealed class MainMenu : ScreenBase
    {
        [SerializeField]
        private ShopButton _startGameButton;

        private GameStateController _gameStateController;

        [Inject]
        private void Construct(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
        }
        
        private void Start()
        {
            _gameStateController.Off();
            EventSystem.current.SetSelectedGameObject(_startGameButton.gameObject);
        }
    }
}