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
            
            public override Location Create(Location prefab)
            {
                var location = Instantiate(prefab);
                _container.Inject(location);

                foreach (var monoBehaviour in location.GetComponentsInChildren<MonoBehaviour>())
                {
                    _container.Inject(monoBehaviour);
                }
                
                return location;
            }
        }
    }
}