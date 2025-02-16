using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class VillagerShopPresenter : ShopPresenterBase
    {
        private readonly InitData _initData;

        [Serializable]
        public struct InitData
        {
            public Product[] Products;
        }

        [Serializable]
        public struct SaveData
        {
            public List<ProductSaveData> ProductSavesData;
        }

        public sealed class Factory : PlaceholderFactory<VillagerShopPresenter> { }

        public VillagerShopPresenter(ShopView shopViewPrefab, ShopButton shopButtonPrefab,
            Dictionary<string, string> inscriptionsContainer, GameStateController gameStateController, 
            TransitionScreen transitionScreen, CharacterInventory characterInventory, WalletService walletService, 
            DiContainer container, AudioClip music, InitData data, ShopBackground background) : base(shopViewPrefab, shopButtonPrefab,
            inscriptionsContainer, gameStateController, transitionScreen, characterInventory, walletService, 
            container, music, background)
        {
            _initData = data;
            
            var speakData = new SpeakData[]
            {
                new("Villager_Speak_1_Name", new[] {"Villager_Speak_1"}),
            };
            
            Init(shopButtonPrefab, speakData, data.Products);
        }

        protected override void Load()
        {
            if (!RepositoryStorage.TryGet(nameof(SaveData), out SaveData saveData))
                return;
            
            foreach (var product in _initData.Products)
            {
                foreach (var productSaveData in saveData.ProductSavesData)
                {
                    if (product.Config.GetID == productSaveData.Id)
                        product.Counts = productSaveData.Counts;
                }
            }
        }

        protected override void Save()
        {
            var saveData = new SaveData
            {
                ProductSavesData = new List<ProductSaveData>()
            };

            foreach (var productData in _initData.Products)
            {
                saveData.ProductSavesData.Add(new ProductSaveData() {Id = productData.Config.GetID,
                    Counts = productData.Counts});
            }
            
            RepositoryStorage.Set(nameof(SaveData), saveData);
        }
    }
}