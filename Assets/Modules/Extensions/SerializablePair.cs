using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public struct SerializablePair<TKey, TValue>
    {
        [field: SerializeField] public TKey Key { get; private set; }
        [field: SerializeField] public TValue Value { get; private set; }

        public SerializablePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}