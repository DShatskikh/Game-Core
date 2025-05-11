using System.Collections.Generic;
using PixelCrushers;
using Sirenix.Utilities;
using UnityEngine;
using YG;

namespace Game
{
    public static class RepositoryStorage
    {
        private static Dictionary<string, string> _container = new();

        public static void Set<T>(string id, T saveData)
        {
            Debug.Log($"string: {id} T: {saveData} Serialize: {SaveSystem.Serialize(saveData)}");
            _container[id] = SaveSystem.Serialize(saveData);
        }

        public static T Get<T>(string id)
        {
            if (!_container.ContainsKey(id))
                return default;

            Debug.Log($"string: {id} T: {_container[id]} Serialize: {SaveSystem.Deserialize<T>(_container[id])}");
            return SaveSystem.Deserialize<T>(_container[id]);
        }
        
        public static bool TryGet<T>(string id, out T value)
        {
            if (!_container.ContainsKey(id))
            {
                value = default;
                return false;
            }
            
            value = SaveSystem.Deserialize<T>(_container[id]);
            return true;
        }
    }
}