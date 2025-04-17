using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Data/Item", order = 220)]
    public sealed class ItemConfig : ScriptableObject
    {
        public Item Prototype;
    }
}