using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class ShopPresenter_Villager : ShopPresenterBase
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

        public sealed class Factory : PlaceholderFactory<ShopPresenter_Villager> { }

        public ShopPresenter_Villager(ShopView shopViewPrefab, ShopButton shopButtonPrefab,
            Dictionary<string, string> inscriptionsContainer, GameStateController gameStateController, 
            MainInventory mainInventory, WalletService walletService, 
            DiContainer container, StudioEventEmitter studioEmitter, InitData data, ShopBackground background) : base(shopViewPrefab, shopButtonPrefab,
            inscriptionsContainer, gameStateController, mainInventory, walletService, 
            container, studioEmitter, background)
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
            if (!RepositoryStorage.TryGet(nameof(OpenShop_Villager), out SaveData saveData))
                return;
            
            foreach (var product in _initData.Products)
            {
                foreach (var productSaveData in saveData.ProductSavesData)
                {
                    if (product.Config.Prototype.ID == productSaveData.Id)
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
                saveData.ProductSavesData.Add(new ProductSaveData() {Id = productData.Config.Prototype.ID,
                    Counts = productData.Counts});
            }
            
            RepositoryStorage.Set(nameof(OpenShop_Villager), saveData);
        }
    }
}