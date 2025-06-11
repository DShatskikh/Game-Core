using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine.EventSystems;
using Zenject;

namespace Game
{
    public sealed class ShopPresenter_Notch_Tutorial : ShopPresenterBase
    {
        private readonly InitData _initData;
        private readonly TutorialState _tutorialState;

        // Структура для работы магазина
        [Serializable]
        public struct InitData
        {
            public Product[] Products;
            public DialogueSystemTrigger BuyDialogue;
        }
        
        public sealed class Factory : PlaceholderFactory<ShopPresenter_Notch_Tutorial> { }

        public ShopPresenter_Notch_Tutorial(ShopView shopViewPrefab, 
            ShopButton shopButtonPrefab, Dictionary<string, string> inscriptionsContainer, 
            GameStateController gameStateController, MainInventory mainInventory, WalletService walletService, 
            DiContainer container, ShopBackground backgroundPrefab, ScreenManager screenManager, 
            MainRepositoryStorage mainRepositoryStorage, InitData initData, TutorialState tutorialState)
            : base(shopViewPrefab, shopButtonPrefab, inscriptionsContainer, 
            gameStateController, mainInventory, walletService, container, backgroundPrefab, screenManager,
            mainRepositoryStorage)
        {
            _tutorialState = tutorialState;
            _initData = initData;
            var products = _initData.Products;
            
            var speakData = new SpeakData[]
            {
                new("Notch_Speak_1_Name", new[] {"Notch_Speak_1"}),
                new("Notch_Speak_2_Name",new[] {"Notch_Speak_2"})
            };
            
            Init(speakData, products);
            OpenBuyPanel();
            _shopView.GetProductExitButton.gameObject.SetActive(false);
            _productButtons[0].GetLabel.SetText(123.ToString());
        }

        private protected override void OnBuyYesButton(Product product, ShopButton productButton)
        {
            if (_walletService.TrySellMoney(product.Price)) { }
            BuySuccess(product, productButton);
            Close();
            
            _tutorialState.FinishStep();
        }

        private protected override void OnClose()
        {
            _initData.BuyDialogue.OnUse();
        }

        private protected override void OnBuyNoButton()
        {
            _shopView.ToggleProductInfo(true);
            _shopView.ToggleBuy(false);
            EventSystem.current.SetSelectedGameObject(_productButtons[0].gameObject);
        }

        protected override void Load() { }

        protected override void Save() { }
    }
}