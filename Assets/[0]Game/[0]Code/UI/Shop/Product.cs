using System;

namespace Game
{
    // Класс продукта в магазине
    [Serializable]
    public class Product
    {
        public ItemConfig Config;
        public int Price;
        public int Counts = -1;
    }
}