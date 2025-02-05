using System.Collections;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Starter : MonoBehaviour
    {
        private GameStateController _gameStateController;

        [Inject]
        private void Construct(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
        }
        
        public IEnumerator Start()
        {
            yield return null;
            _gameStateController.StartGame();
        }
    }
}