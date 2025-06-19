using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Zenject;

namespace Game
{
    // Меню игры
    public sealed class MainMenu : ScreenBase
    {
        [SerializeField]
        private ShopButton _startGameButton;

        [SerializeField]
        private ShopButton _continueButton;
        
        [SerializeField]
        private GameObject _continueContainer; 
        
        [SerializeField]
        private GameObject _startGameContainer; 

        private GameStateController _gameStateController;
        private IGameRepositoryStorage _gameRepositoryStorage;

        [Inject]
        private void Construct(GameStateController gameStateController, IGameRepositoryStorage gameRepositoryStorage)
        {
            _gameStateController = gameStateController;
            _gameRepositoryStorage = gameRepositoryStorage;
        }
        
        private void Start()
        {
            _gameStateController.Off();
            
            if (_gameRepositoryStorage.TryGet(SaveConstants.STARTED_GAME, out MarkerData markerData))
            {
                _continueContainer.SetActive(true);
                _startGameContainer.SetActive(false);
                EventSystem.current.SetSelectedGameObject(_continueButton.gameObject);
                return;    
            }

            _continueContainer.SetActive(false);
            _startGameContainer.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_startGameButton.gameObject);
        }
    }
}