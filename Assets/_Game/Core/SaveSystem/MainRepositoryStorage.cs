using UnityEngine;
using YG;

namespace Game
{
    // Репозиторий для игрового процесса
    public sealed class MainRepositoryStorage : IGameRepositoryStorage
    {
        private const string SAVE_KEY = "MAIN_SAVE_KEY";
        private SerializableDictionary<string, string> _container = new();

        public void Set<T>(string id, T saveData)
        {
            var value = JsonUtility.ToJson(saveData);
            _container[id] = value;
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
#if YandexGamesPlatform_yg
                var data = JsonUtility.FromJson<SerializableDictionary<string, string>>(YG2.saves.Data) ?? new SerializableDictionary<string, string>();
#else 
                var data = JsonUtility.FromJson<SerializableDictionary<string, string>>(PlayerPrefs.GetString(SAVE_KEY));
#endif

                _container = data;
            }
            else
            {
            }
        }

        public void Save()
        {
            var data = JsonUtility.ToJson(_container);

#if YandexGamesPlatform_yg
            YG2.SaveProgress();
#else 
            PlayerPrefs.SetString(SAVE_KEY, data);
#endif
        }
    }
}