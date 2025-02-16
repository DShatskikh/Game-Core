using System;

namespace Game
{
    [Serializable]
    public class Product
    {
        public ItemBaseConfig Config;
        public int Price;
        public int Counts = -1;
    }
}