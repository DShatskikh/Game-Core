using System;
using UnityEngine;

namespace Game
{
    // Сервис для хранения времени игры
    public sealed class SessionTimeSystem
    {
        private DateTime _startGameTime;
        private TimeSpan _allGameTime;
        private int _currentIndex;
        private readonly IGameRepositoryStorage _mainRepositoryStorage;

        [Serializable]
        public struct SaveData
        {
            public TimeSpan GameTime;
        }
        
        public SessionTimeSystem(IGameRepositoryStorage mainRepositoryStorage)
        {
            _mainRepositoryStorage = mainRepositoryStorage;
            _startGameTime = DateTime.Now;
            Load();
        }
        
        public string GetTextTime()
        {
            var minutes = _allGameTime.Hours * _allGameTime.Minutes + _allGameTime.Minutes < 10 
                ? $"0{_allGameTime.Hours * _allGameTime.Minutes + _allGameTime.Minutes}" 
                : $"{_allGameTime.Hours * _allGameTime.Minutes + _allGameTime.Minutes}";
            
            var seconds = _allGameTime.Seconds < 10 ? $"0{_allGameTime.Seconds}" : $"{_allGameTime.Seconds}";
            return $"{minutes}:{seconds}";
        }

        public void Save()
        {
            var time = DateTime.Now - _startGameTime + _allGameTime;
            _allGameTime = time;
            
            _mainRepositoryStorage.Set(SaveConstants.SESSION_TIME, new SaveData()
            {
                GameTime = time
            });
        }

        private void Load()
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.SESSION_TIME, out SaveData saveData))
            {
                _allGameTime = saveData.GameTime;
            }
        }
    }
}