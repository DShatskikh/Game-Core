using UnityEngine;

namespace Game
{
    public abstract class ItemBaseConfig : ScriptableObject, IItemConfig
    {
        [SerializeField]
        private string _id;

        [SerializeField]
        private string _name;

        public string GetID => _id;
        public string GetName => _name;
        public abstract string GetInfo { get; }
        public GameObject Prefab { get; }
    }
}