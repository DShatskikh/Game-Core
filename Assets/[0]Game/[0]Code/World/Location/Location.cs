using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class Location : MonoBehaviour
    {
        [SerializeField]
        private string _id;
        
        [SerializeField]
        private Transform[] _points;

        public Transform[] GetPoints => _points;
        public string GetID => _id;
        
        // Фабрика для создания локации
        public class Factory : PlaceholderFactory<Location, Location>
        {
            private readonly DiContainer _container;
            private readonly GameStateController _gameStateController;

            public Factory(DiContainer container, GameStateController gameStateController)
            {
                Debug.Log(container);
                Debug.Log(gameStateController);
                
                _container = container;
                _gameStateController = gameStateController;
            }
            
            // Создание уровня
            public override Location Create(Location prefab)
            {
                var location = Instantiate(prefab);
                Debug.Log(location);
                
                // Прокидываем зависимости в компоненты обьекта уровня
                _container.Inject(location);

                // Прокидываем зависимости в дочерние обьекты уровня
                foreach (var monoBehaviour in location.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (monoBehaviour == null)
                        continue;
                    
                    _container.Inject(monoBehaviour);
                    
                    if (monoBehaviour.TryGetComponent<IGameListener>(out var gameListener))
                        _gameStateController.AddListener(gameListener);
                }

                return location;
            }
        }
    }
}