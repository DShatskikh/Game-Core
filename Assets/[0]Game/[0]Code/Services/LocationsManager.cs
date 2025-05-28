using System.Linq;
using UnityEngine;

namespace Game
{
    // Менеджер уровней
    public sealed class LocationsManager
    {
        private readonly Location[] _locations;
        private readonly Player _player;
        private readonly Location.Factory _factory;

        private Location _location;

        public LocationsManager(Location[] locations, Player player, Location.Factory factory)
        {
            _locations = locations;
            _player = player;
            _factory = factory;
        }
        
        // Переходим на уровень по id
        public void SwitchLocation(string id, int pointIndex)
        {
            if (!_location)
                _location = Object.FindFirstObjectByType<Location>();
            
            if (_location)
                Object.Destroy(_location.gameObject);
            
            var prefab = _locations.FirstOrDefault(x => x.GetID == id);
            
            if (prefab == null)
                return;
            
            _location = _factory.Create(prefab);
            _player.transform.position = _location.GetPoints[pointIndex].transform.position;
        }
    }
}