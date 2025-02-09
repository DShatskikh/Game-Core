using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class GameKernel : MonoKernel
    {
        [Inject]
        private GameStateController _gameStateController;
        
        [InjectLocal]
        private List<IGameListener> _listeners;

        public override void Start()
        {
            base.Start();
            
            _listeners.AddRange(FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IGameListener>());

            foreach (var listener in _listeners) 
                _gameStateController.AddListener(listener);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            foreach (var listener in _listeners) 
                _gameStateController.RemoveListener(listener);
        }
    }
}