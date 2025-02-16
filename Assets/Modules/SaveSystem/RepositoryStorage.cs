using System.Collections.Generic;
using PixelCrushers;
using YG;

namespace Game
{
    public static class RepositoryStorage
    {
        public static List<SerializablePair<string, string>> Container => YG2.saves.Container;

        public static void Set<T>(string id, T saveData)
        {
            if (Container == null)
                YG2.saves.Container = new List<SerializablePair<string, string>>();

            var value = SaveSystem.Serialize(saveData);
            
            for (int i = 0; i < Container.Count; i++)
            {
                var pair = Container[i];

                if (id == pair.Key)
                {
                    YG2.saves.Container[i] = new SerializablePair<string, string>(id, value);  
                    return;
                }
            }
            
            YG2.saves.Container.Add(new SerializablePair<string, string>(id, value));
        }

        public static T Get<T>(string id) where T : new()
        {
            if (Container == null)
                YG2.saves.Container = new List<SerializablePair<string, string>>();
            
            for (int i = 0; i < Container.Count; i++)
            {
                var pair = Container[i];

                if (id == pair.Key)
                    return SaveSystem.Deserialize<T>(YG2.saves.Container[i].Value);
            }
            
            return new T();
        }
        
        public static bool TryGet<T>(string id, out T value) where T : new()
        {
            if (Container == null)
                YG2.saves.Container = new List<SerializablePair<string, string>>();
            
            for (int i = 0; i < Container.Count; i++)
            {
                var pair = Container[i];

                if (id == pair.Key)
                {
                    value = SaveSystem.Deserialize<T>(YG2.saves.Container[i].Value);
                    return true;
                }
            }

            value = new T();
            return false;
        }
    }
}