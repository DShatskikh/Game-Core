using System;

namespace Game
{
    [Serializable]
    public class Product
    {
        public ItemConfig Config;
        public int Price;
        public int Counts = -1;
    }
}