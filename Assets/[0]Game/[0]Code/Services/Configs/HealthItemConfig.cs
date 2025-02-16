using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "HealthItem", menuName = "Data/Items/HealthItem", order = 85)]
    public class HealthItemConfig : ItemBaseConfig
    {
        public int Health;
        public override string GetInfo => $"Лечит {Health}ОЗ";
    }
}