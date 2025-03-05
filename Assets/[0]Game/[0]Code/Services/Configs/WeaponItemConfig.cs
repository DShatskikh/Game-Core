using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "WeaponItem", menuName = "Data/Items/WeaponItem", order = 87)]
    public class WeaponItemConfig : ItemBaseConfig
    {
        public int Attack;
        public GameObject Effect;
        public override string GetInfo => $"+{Attack} АТК";
    }
}