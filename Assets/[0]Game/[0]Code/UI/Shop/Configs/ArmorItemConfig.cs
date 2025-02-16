using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ArmorItem", menuName = "Data/Items/ArmorItem", order = 86)]
    public class ArmorItemConfig : ItemBaseConfig
    {
        public int Protection;
        public override string GetInfo => $"+{Protection} ЗЩ";
    }
}