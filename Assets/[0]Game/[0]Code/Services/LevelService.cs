using System;

namespace Game
{
    public sealed class LevelService
    {
        public const int MAX_LEVEL = 7;
        public const int ADD_HEALTH = 4;

        private readonly MainRepositoryStorage _mainRepositoryStorage;
        private readonly HealthService _healthService;
        
        private int _previousLv;
        private int _lv;
        private int _exp;
        private int _expToNextLv;

        public int GetLv => _lv;
        public int GetPreviousLv => _previousLv;
        public int GetExp => _exp;
        public int GetExpToNextLv => _expToNextLv;
        
        [Serializable]
        public struct SaveData
        {
            public int PreviousLv;
            public int Lv;
            public int Exp;
            public int ExpToNextLv;
        }
        
        public LevelService(MainRepositoryStorage mainRepositoryStorage, HealthService healthService)
        {
            _mainRepositoryStorage = mainRepositoryStorage;
            _healthService = healthService;

            _lv = 0;
            _exp = 0;
            _expToNextLv = GetNextToNextLv(0);
            
            if (_mainRepositoryStorage.TryGet(SaveConstants.LV, out SaveData saveData))
            {
                _lv = saveData.Lv;
                _exp = saveData.Exp;
                _expToNextLv = saveData.ExpToNextLv;
                
                _healthService.AddMaxHealth(ADD_HEALTH);
                _healthService.Reset();
            }

            _previousLv = _lv;
        }
        
        public void AddExp(int value, out bool isLevelUp)
        {
            _exp += value;

            while (_exp >= _expToNextLv && _lv < MAX_LEVEL)
            {
                _exp %= _expToNextLv;
                _lv++;
                _expToNextLv = GetNextToNextLv(_lv);
                isLevelUp = true;
            }

            if (_lv >= MAX_LEVEL)
            {
                _exp = 0;
                _expToNextLv = 0;
            }
            
            Save();
            isLevelUp = false;
        }

        public void SaveGame()
        {
            _previousLv = _lv;
        }
        
        private int GetNextToNextLv(int nextLv)
        {
            nextLv++;
            
            return nextLv switch
            {
                1 => 100,
                2 => 125,
                3 => 200,
                4 => 250,
                5 => 400,
                6 => 750,
                7 => 1000,
                _ => 0
            };
        }

        private void Save()
        {
            _mainRepositoryStorage.Set(SaveConstants.HEALTH, new SaveData()
            {
                Lv = _lv,
                Exp = _exp,
                ExpToNextLv = _expToNextLv,
                PreviousLv = _lv
            });
        }
    }
}