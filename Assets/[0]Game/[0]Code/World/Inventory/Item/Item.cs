using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [Serializable]
    public sealed class Item
    {
        [FormerlySerializedAs("Id")]
        public string ID;
        public ItemMetaData MetaData;
        public ItemFlags Flags;
        
        [SerializeReference]
        public IItemComponent[] Components;

        public Item(string id, ItemMetaData metaData, ItemFlags flags, IItemComponent[] components)
        {
            ID = id;
            MetaData = metaData;
            Flags = flags;
            Components = new IItemComponent[components.Length];

            for (int i = 0; i < components.Length; i++)
            {
                if ( components[i] == null)
                    continue;
                
                Components[i] = components[i].Clone();
            }
        }
        
        public Item Clone() => 
            new(ID, MetaData.Clone(), Flags, Components);

        public bool TryGetComponent<T>(out T component) where T : IItemComponent
        {
            foreach (var itemComponent in Components)
            {
                if (itemComponent is T result)
                {
                    component = result;
                    return true;
                }
            }

            component = default;
            return false;
        }
    }
}