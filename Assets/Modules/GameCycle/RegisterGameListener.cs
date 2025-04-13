using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class RegisterGameListener : MonoBehaviour
    {
        [Inject]
        private GameStateController _gameStateController;
        
        [InjectLocal]
        private List<IGameListener> _listeners;

        public void Awake()
        {
            _listeners.AddRange(FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IGameListener>());

            foreach (var listener in _listeners) 
                _gameStateController.AddListener(listener);
        }

        public void OnDestroy()
        {
            foreach (var listener in _listeners) 
                _gameStateController.RemoveListener(listener);
        }
    }
}