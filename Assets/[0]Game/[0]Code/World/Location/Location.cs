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
        
        public class Factory : PlaceholderFactory<Location, Location>
        {
            private readonly DiContainer _container;

            public Factory(DiContainer container)
            {
                _container = container;
            }
            
            // Создание уровня
            public override Location Create(Location prefab)
            {
                var location = Instantiate(prefab);
                
                // Прокидываем зависимости в компоненты обьекта уровня
                _container.Inject(location);

                // Прокидываем зависимости в дочерние обьекты уровня
                foreach (var monoBehaviour in location.GetComponentsInChildren<MonoBehaviour>(true))
                    _container.Inject(monoBehaviour);

                return location;
            }
        }
    }
}