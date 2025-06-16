using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Zenject;

namespace Game
{
    public sealed class ShopPresenter_Bisnesman : ShopPresenterBase
    {
        private readonly InitData _initData;

        // Структура для работы магазина
        [Serializable]
        public struct InitData
        {
            public Product[] Products;
            public Product SecretKey;
        }

        // Структура для сохранения данных магазина
        [Serializable]
        public struct SaveData
        {
            public List<ProductSaveData> ProductsData;
            public List<SpeakData> SpeaksData;
        }

        // Фабрика для создания магазина
        public sealed class Factory : PlaceholderFactory<ShopPresenter_Bisnesman> { }

        
        public ShopPresenter_Bisnesman(ShopView shopViewPrefab, ShopButton shopButtonPrefab,
            Dictionary<string, string> inscriptionsContainer, GameStateController gameStateController,
            MainInventory mainInventory, WalletService walletService, DiContainer container, 
            ShopBackground backgroundPrefab, ScreenManager screenManager, IGameRepositoryStorage mainRepositoryStorage,
            InitData data) : base(shopViewPrefab, shopButtonPrefab, inscriptionsContainer, gameStateController, 
            mainInventory, walletService, container, backgroundPrefab, screenManager, mainRepositoryStorage)
        {
            var products = data.Products.ToList();
            products.Add(data.SecretKey);
            
            var speakData = new SpeakData[]
            {
                new("Notch_Speak_1_Name", new[] {"Notch_Speak_1"}),
                new("Notch_Speak_2_Name",new[] {"Notch_Speak_2"})
            };
            
            Init(speakData, products.ToArray());
        }

        protected override void Load()
        {
            
        }

        protected override void Save()
        {
            
        }
    }
}