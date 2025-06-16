using System;
using Sirenix.OdinInspector;

namespace Game
{
    // Класс продукта в магазине
    [Serializable]
    public class Product
    {
        private bool _isNotDonation => !IsDonation;
        
        public ItemConfig Config;
        public bool IsDonation;

        [ShowIf("_isNotDonation", false)]
        public int Price;
        
        [ShowIf("IsDonation", false)]
        public int DonationPrice;
        
        public int Counts = -1;
    }
}