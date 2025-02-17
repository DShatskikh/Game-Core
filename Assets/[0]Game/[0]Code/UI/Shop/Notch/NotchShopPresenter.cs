using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class NotchShopPresenter : ShopPresenterBase
    {
        private readonly InitData _initData;

        [Serializable]
        public struct InitData
        {
            public Product[] Swords;
            public Product[] Armors;
            public Product[] Products;
        }

        [Serializable]
        public struct SaveData
        {
            public List<ProductSaveData> ProductsData;
            public List<SpeakData> SpeaksData;
        }

        public sealed class Factory : PlaceholderFactory<NotchShopPresenter> { }

        public NotchShopPresenter(ShopView shopViewPrefab, ShopButton shopButtonPrefab,
            Dictionary<string, string> inscriptionsContainer, GameStateController gameStateController, 
            TransitionScreen transitionScreen, CharacterInventory characterInventory, WalletService walletService, 
            DiContainer container, AudioClip music, InitData data, ShopBackground shopBackground) : base(shopViewPrefab, shopButtonPrefab,
            inscriptionsContainer, gameStateController, transitionScreen, characterInventory, walletService, 
            container, music, shopBackground)
        {
            _initData = data;
            
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
            
            Init(shopButtonPrefab, speakData, products);
        }

        protected override void BuySuccess(Product product, ShopButton productButton)
        {
            _characterInventory.AddItem(product.Config);
            _shopView.SetStatsText(_inscriptionsContainer["BuySuccess"]);
            SoundPlayer.Play(AssetProvider.Instance.BuySound);

            var sword = _initData.Swords.FirstOrDefault(sword => sword.Config.GetID == product.Config.GetID);
            
            if (sword != null)
            {
                var index = _initData.Swords.IndexOf(sword);

                if (index != _initData.Swords.Length - 1)
                {
                    var nextSword = _initData.Swords[index + 1];
                    ReplaceProduct(sword, nextSword);
                    return;
                }
            }
            
            var armor = _initData.Armors.FirstOrDefault(armor => armor.Config.GetID == product.Config.GetID);
            
            if (armor != null)
            {
                var index = _initData.Armors.IndexOf(armor);

                if (index != _initData.Armors.Length - 1)
                {
                    var nextArmor = _initData.Armors[index + 1];
                    ReplaceProduct(armor, nextArmor);
                    return;
                }
            }
            
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
            if (!RepositoryStorage.TryGet(nameof(SaveData), out SaveData saveData))
                return;

            var allProducts = new List<Product>(_initData.Products);
            allProducts.AddRange( _initData.Armors);
            allProducts.AddRange( _initData.Swords);

            var currentProducts = new List<Product>();
            
            foreach (var productSaveData in saveData.ProductsData)
            {
                foreach (var product in allProducts.Where(product => product.Config.GetID == productSaveData.Id))
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
                saveData.ProductsData.Add(new ProductSaveData() {Id = productData.Config.GetID,
                    Counts = productData.Counts});
            }

            saveData.SpeaksData = _speakData;
            
            RepositoryStorage.Set(nameof(SaveData), saveData);
        }
    }
}