using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
    // Менеджер уровней
    public sealed class LocationsManager
    {
        [Serializable]
        public struct Data
        {
            public string LocationId;
            public int PointIndex;
            public Vector2 PlayerPosition;
        }
        
        private readonly Location[] _locations;
        private readonly Player _player;
        private readonly Location.Factory _factory;
        private readonly IGameRepositoryStorage _gameRepositoryStorage;

        private Location _location;

        public LocationsManager(Location[] locations, Player player, Location.Factory factory,
            IGameRepositoryStorage gameRepositoryStorage)
        {
            _locations = locations;
            _player = player;
            _factory = factory;
            _gameRepositoryStorage = gameRepositoryStorage;
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
            
            _gameRepositoryStorage.Set(SaveConstants.LOCATION, new Data()
            {
                LocationId = id,
                PointIndex = pointIndex,
                PlayerPosition = _player.transform.position
            });
        }
    }
}