using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Item", menuName = "Data/Items/Item", order = 84)]
    public class ItemConfig : ItemBaseConfig
    {
        [SerializeField]
        private string _info;
        
        public override string GetInfo { get; }
    }
}