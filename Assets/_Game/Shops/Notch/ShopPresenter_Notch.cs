﻿using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Game
{
    // Логика открытия магазина Нотча
    public sealed class ShopPresenter_Notch : ShopPresenterBase
    {
        private readonly InitData _initData;

        // Структура для работы магазина
        [Serializable]
        public struct InitData
        {
            public Product[] Swords;
            public Product[] Armors;
            public Product[] Products;
        }

        // Структура для сохранения данных магазина
        [Serializable]
        public struct SaveData
        {
            public List<ProductSaveData> ProductsData;
            public List<SpeakData> SpeaksData;
        }

        // Фабрика для создания магазина
        public sealed class Factory : PlaceholderFactory<ShopPresenter_Notch> { }

        public ShopPresenter_Notch(ShopView shopViewPrefab, ShopButton shopButtonPrefab,
            Dictionary<string, string> inscriptionsContainer, GameStateController gameStateController, 
            MainInventory mainInventory, WalletService walletService, 
            DiContainer container, InitData data, ShopBackground shopBackgroundPrefab,
            ScreenManager screenManager, IPurchaseService purchaseService, IGameRepositoryStorage mainRepositoryStorage) : base(shopViewPrefab, shopButtonPrefab,
            inscriptionsContainer, gameStateController, mainInventory, walletService, 
            container, shopBackgroundPrefab, screenManager, mainRepositoryStorage, purchaseService)
        {
            _initData = data;
            Load();
            
            var products = new[]
            {
                data.Swords[0],
                data.Armors[0],
                data.Products[0],
            };
            
            var speakData = new SpeakData[]
            {
                new("Notch_Speak_1_Name", new[] {"Notch_Speak_1"}),
                new("Notch_Speak_2_Name",new[] {"Notch_Speak_2"})
            };
            
            Init(speakData, products);
        }

        protected override void BuySuccess(Product product, ShopButton productButton)
        {
            _shopView.SetStatsText(_inscriptionsContainer["BuySuccess"]);
            PlayBuySound();

            var sword = _initData.Swords.FirstOrDefault(sword => sword.Config.Prototype.ID == product.Config.Prototype.ID);
            
            if (sword != null)
            {
                _mainInventory.EquipWeapon(sword.Config.Prototype.Clone());
                
                var index = _initData.Swords.IndexOf(sword);

                if (index != _initData.Swords.Length - 1)
                {
                    var nextSword = _initData.Swords[index + 1];
                    ReplaceProduct(sword, nextSword);
                    return;
                }
            }
            
            var armor = _initData.Armors.FirstOrDefault(armor => armor.Config.Prototype.ID == product.Config.Prototype.ID);
            
            if (armor != null)
            {
                _mainInventory.EquipArmor(armor.Config.Prototype.Clone());
                
                var index = _initData.Armors.IndexOf(armor);

                if (index != _initData.Armors.Length - 1)
                {
                    var nextArmor = _initData.Armors[index + 1];
                    ReplaceProduct(armor, nextArmor);
                    return;
                }
            }
            
            if (sword == null && armor == null)
                _mainInventory.Add(product.Config.Prototype);
            
            if (product.Counts > 0)
                product.Counts -= 1;

            if (product.Counts == 0)
                ProductButtonInactive(productButton);
        }

        protected override void InitSpeakButton(ShopButton speakButton, SpeakData pair)
        {
            if (pair.NameID == "Notch_Speak_2_Name")
            {
                speakButton.onClick.AddListener(() =>
                {
                    if (_speakData.Any(data => data.NameID == "Notch_Speak_3_Name"))
                        return;

                    var createPair = new SpeakData()
                    {
                        NameID = "Notch_Speak_3_Name",
                        ResponceID = new[] { "Notch_Speak_3" }
                    };
                    
                    CreateSpeakButton(createPair);
                    _speakData.Add(createPair);
                    Debug.Log("Создали новый диалог");
                });
            }
            
            base.InitSpeakButton(speakButton, pair);
        }

        protected override void Load()
        {
            if (!_mainRepositoryStorage.TryGet(nameof(ShopPresenter_Notch), out SaveData saveData))
                return;

            var allProducts = new List<Product>(_initData.Products);
            allProducts.AddRange( _initData.Armors);
            allProducts.AddRange( _initData.Swords);

            var currentProducts = new List<Product>();
            
            foreach (var productSaveData in saveData.ProductsData)
            {
                foreach (var product in allProducts.Where(product => product.Config.Prototype.ID == productSaveData.Id))
                {
                    product.Counts = productSaveData.Counts;
                    currentProducts.Add(product);
                }
            }

            _speakData = saveData.SpeaksData;
            _products = currentProducts.ToArray();
        }

        protected override void Save()
        {
            var saveData = new SaveData
            {
                ProductsData = new List<ProductSaveData>(),
            };
            
            foreach (var productData in _products)
            {
                saveData.ProductsData.Add(new ProductSaveData() {Id = productData.Config.Prototype.ID,
                    Counts = productData.Counts});
            }

            saveData.SpeaksData = _speakData;
            
            _mainRepositoryStorage.Set(nameof(ShopPresenter_Notch), saveData);
        }
    }
}