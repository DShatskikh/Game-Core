using UnityEngine;

namespace Game
{
    public sealed class SettingsRepositoryStorage : IGameRepository
    {
        private const string SAVE_KEY = "SETTINGS_KEY";
        private SerializableDictionary<string, string> _container = new();

        public void Set<T>(string key, T saveData)
        {
            var data = JsonUtility.ToJson(saveData);
            _container[key] = data;
        }

        public bool TryGet<T>(string key, out T data)
        {
            if (_container.TryGetValue(key, out string stringValue))
            {
                data = JsonUtility.FromJson<T>(stringValue);
                return true;
            }

            data = default;
            return false;
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                var data = JsonUtility.FromJson<SerializableDictionary<string, string>>(PlayerPrefs.GetString(SAVE_KEY));
                _container = data;
            }
            else
            {
            }
        }

        public void Save()
        {
            var data = JsonUtility.ToJson(_container);
            PlayerPrefs.SetString(SAVE_KEY, data);
        }
    }
}