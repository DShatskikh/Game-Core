using System;
using UniRx;

namespace Game
{
    public sealed class HealthService
    {
        private readonly MainRepositoryStorage _mainRepositoryStorage;
        private IntReactiveProperty _health = new();
        private IntReactiveProperty _maxHealth = new();
        
        public IReactiveProperty<int> GetHealth => _health;
        public IReactiveProperty<int> GetMaxHealth => _maxHealth;

        [Serializable]
        public struct SaveData
        {
            public int MaxHealth;
            public int Health;
        }
        
        public HealthService(MainRepositoryStorage mainRepositoryStorage)
        {
            _mainRepositoryStorage = mainRepositoryStorage;

            _maxHealth.Value = 20;
            _health.Value = 20;
            
            if (_mainRepositoryStorage.TryGet(SaveConstants.HEALTH, out SaveData saveData))
            {
                _maxHealth.Value = saveData.MaxHealth;
                _health.Value = saveData.Health;
            }
        }

        public void Add(int value)
        {
            if (_health.Value + value > _maxHealth.Value)
            {
                _health.Value = _maxHealth.Value;
                return;
            }

            _health.Value += value;
            Save();
        }

        public void Sell(int value)
        {
            if (_health.Value - value <= 0)
            {
                _health.Value = 0;
                return;
            }

            _health.Value -= value;
            Save();
        }
        
        public void AddMaxHealth(int value)
        {
            _maxHealth.Value += value;
            Save();
        }
        
        public void Reset()
        {
            _health.Value = _maxHealth.Value;
        }

        private void Save()
        {
            _mainRepositoryStorage.Set(SaveConstants.HEALTH, new SaveData()
            {
                Health = _health.Value,
                MaxHealth = _maxHealth.Value
            });
        }
    }
}