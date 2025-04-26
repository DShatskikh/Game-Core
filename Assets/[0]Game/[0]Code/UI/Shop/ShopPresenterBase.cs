using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    public abstract class ShopPresenterBase : IDisposable
    {
        protected readonly ShopView _shopView;
        private readonly GameStateController _gameStateController;
        protected readonly MainInventory _mainInventory;
        private readonly WalletService _walletService;
        protected readonly Dictionary<string, string> _inscriptionsContainer;
        private readonly ShopButton _shopButtonPrefab;
        private readonly ScreenManager _screenManager;

        protected List<ShopButton> _productButtons = new();
        private List<ShopButton> _speakButtons = new();
        private List<ShopButton> _actButtons = new();
        protected Product[] _products;
        protected List<SpeakData> _speakData = new();

        public ShopPresenterBase(ShopView shopViewPrefab, ShopButton shopButtonPrefab,
            Dictionary<string, string> inscriptionsContainer, GameStateController gameStateController, 
            MainInventory mainInventory, WalletService walletService, 
            DiContainer container, StudioEventEmitter studioEmitter, ShopBackground backgroundPrefab, 
            ScreenManager screenManager)
        {
            _shopView = Object.Instantiate(shopViewPrefab);
            _inscriptionsContainer = inscriptionsContainer;
            _gameStateController = gameStateController;
            _mainInventory = mainInventory;
            _walletService = walletService;
            _shopButtonPrefab = shopButtonPrefab;
            _screenManager = screenManager;

            _gameStateController.OpenDialog();
            studioEmitter.Play();

            container.Inject(_shopView.GetMonologueText);

            var background = Object.Instantiate(backgroundPrefab, _shopView.GetBackgroundContainer);
            //container.Inject(background);
        }

        protected virtual void InitProducts(Product[] products) => 
            _products = products;

        protected void Init(ShopButton shopButtonPrefab, SpeakData[] speakData, Product[] products)
        {
            InitProducts(products);
            InitSpeaks(speakData);
            Load();
            InitSpeakButtons();
            InitProductButtons();
            InitActButtons();
            InitItemCountText();
            InitMoneyText();

            OpenStartPanel();
        }

        private void InitSpeaks(SpeakData[] speakData) => 
            _speakData = speakData.ToList();

        protected abstract void Load();
        protected abstract void Save();
        
        protected void InitMoneyText()
        {
            _walletService.Money.SubscribeAndCall(money => _shopView.SetMoneyText($"{money}М"));
        }

        protected void InitItemCountText()
        {
            _shopView.SetItemCountText($"{_mainInventory.MainSlots.Length}/{_mainInventory.MainSlots.Length}");
            _mainInventory.OnSlotChange += (item, slot) => _shopView.SetItemCountText($"{_mainInventory.MainSlots.Length}/{_mainInventory.MainSlots.Length}");
        }
        
        private void CloseAllPanel()
        {
            _shopView.ToggleSelectPanel(false);
            _shopView.ToggleMonologueText(false);
            _shopView.ToggleProducts(false);
            _shopView.ToggleStats(false);
            _shopView.ToggleActs(false);
            _shopView.ToggleSelectInfoText(false);
            _shopView.ToggleSpeakContainer(false);
            _shopView.ToggleBuy(false);
        }
        
        protected void OpenStartPanel()
        {
            CloseAllPanel();
            _shopView.ToggleSelectPanel(true);
            _shopView.ToggleActs(true);
            _shopView.ToggleSelectInfoText(true);
            _shopView.SetSelectInfoText(_inscriptionsContainer["Welcome"]);
            
            EventSystem.current.SetSelectedGameObject(_actButtons[0].gameObject);
        }

        protected void ProductButtonInactive(ShopButton productButton)
        {
            productButton.GetLabel.text = "--Продано--";
            productButton.GetLabel.color = new Color(1, 1, 1, 0.5f);
            productButton.interactable = false;
        }
        
        private void OnExitClicked()
        {
            CloseAllPanel();
            _shopView.GetMonologueText.StartWrite(new[] { _inscriptionsContainer["Farewell"] }, Close);
            Save();
        }

        private void OnSpeakClicked()
        {
            CloseAllPanel();
            _shopView.ToggleSelectPanel(true);
            _shopView.ToggleSpeakContainer(true);
            _shopView.ToggleStats(true);
            _shopView.SetStatsText(_inscriptionsContainer["SpeakStats"]);
            
            EventSystem.current.SetSelectedGameObject(_speakButtons[0].gameObject);
        }

        private void OnSellClicked()
        {
            CloseAllPanel();
            _shopView.GetMonologueText.StartWrite(
                new[] { _inscriptionsContainer["SellNotCan"] }, OpenStartPanel);
        }

        private void OnBuyClicked()
        {
            CloseAllPanel();
            _shopView.ToggleSelectPanel(true);
            _shopView.ToggleStats(true);
            _shopView.ToggleProducts(true);
            _shopView.SetStatsText(_inscriptionsContainer["BuyStats"]);

            EventSystem.current.SetSelectedGameObject(_productButtons[0].gameObject);
        }
        
        protected virtual void InitProductButtons()
        {
            foreach (var product in _products)
            {
                var productButton = Object.Instantiate(_shopButtonPrefab, _shopView.GetProductContainer);
                InitProductButton(productButton, product);
                _productButtons.Add(productButton);
            }

            _shopView.GetProductExitButton.onClick.AddListener(() =>
            {
                OpenStartPanel();
            });
            
            ((ShopButton)_shopView.GetProductExitButton).OnSelectAction += () => { _shopView.ToggleProductInfo(false); };
        }

        protected void ReplaceProduct(Product current, Product next)
        {
            var index = Array.IndexOf(_products, current);
            _products[index] = next;

            var siblingIndex = _productButtons[index].transform.GetSiblingIndex();

            Object.Destroy(_productButtons[index].gameObject);
            _productButtons.Remove(_productButtons[index]);

            var productButton = Object.Instantiate(_shopButtonPrefab, _shopView.GetProductContainer);
            productButton.transform.SetSiblingIndex(siblingIndex);
            InitProductButton(productButton, next);
            _productButtons.Insert(index, productButton);
        }
        
        private void InitProductButton(ShopButton productButton, Product product)
        {
            productButton.GetLabel.text = $"{product.Price}М - {product.Config.Prototype.MetaData.Name}";
            productButton.OnSelectAction += () =>
            {
                _shopView.ToggleProductInfo(true);
                _shopView.SetProductInfoText(product.Config.Prototype.MetaData.Description);
            };

            productButton.onClick.AddListener(() =>
            {
                _shopView.ToggleProductInfo(false);
                _shopView.ToggleStats(false);
                _shopView.ToggleBuy(true);
                _shopView.SetBuyText($"Купить за {product.Price}М");
                EventSystem.current.SetSelectedGameObject(_shopView.GetBuyYesButton.gameObject);

                _shopView.GetBuyYesButton.onClick.RemoveAllListeners();
                _shopView.GetBuyYesButton.onClick.AddListener(() =>
                {
                    EventSystem.current.SetSelectedGameObject(_shopView.GetProductExitButton.gameObject);
                    _shopView.ToggleBuy(false);
                    _shopView.ToggleStats(true);

                    if (_walletService.TrySellMoney(product.Price))
                    {
                        BuySuccess(product, productButton);
                    }
                    else
                    {
                        _shopView.SetStatsText(_inscriptionsContainer["BuyFail"]);
                        PlayFailSound();
                    }
                });

                _shopView.GetBuyNoButton.onClick.RemoveAllListeners();
                _shopView.GetBuyNoButton.onClick.AddListener(() =>
                {
                    _shopView.ToggleProductInfo(true);
                    _shopView.ToggleBuy(false);
                    EventSystem.current.SetSelectedGameObject(_shopView.GetProductExitButton.gameObject);
                });
            });

            if (product.Counts == 0)
                ProductButtonInactive(productButton);
        }

        protected virtual void BuySuccess(Product product, ShopButton productButton)
        {
            _mainInventory.Add(product.Config.Prototype.Clone());
            _shopView.SetStatsText(_inscriptionsContainer["BuySuccess"]);

            if (product.Counts > 0)
                product.Counts -= 1;

            if (product.Counts == 0)
                ProductButtonInactive(productButton);
        }

        protected virtual void InitSpeakButtons()
        {
            foreach (var pair in _speakData)
            {
                CreateSpeakButton(pair);
            }

            _shopView.GetSpeakExitButton.onClick.AddListener(() =>
            {
                OpenStartPanel();
            });
        }

        protected void CreateSpeakButton(SpeakData pair)
        {
            var speakButton = Object.Instantiate(_shopButtonPrefab, _shopView.GetSpeakContainer);
            InitSpeakButton(speakButton, pair);
            _speakButtons.Add(speakButton);
        }

        protected virtual void InitSpeakButton(ShopButton speakButton, SpeakData pair)
        {
            speakButton.GetLabel.text = _inscriptionsContainer[pair.NameID];
            speakButton.onClick.AddListener(() =>
            {
                var responce = new List<string>();

                foreach (var id in pair.ResponceID) 
                    responce.Add(_inscriptionsContainer[id]);

                _shopView.GetMonologueText.StartWrite(responce.ToArray(), OpenStartPanel);
                _shopView.ToggleSpeakContainer(false);
                _shopView.ToggleStats(false);
                _shopView.ToggleSelectPanel(false);
            });
        }

        private void InitActButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                var actButton = Object.Instantiate(_shopButtonPrefab, _shopView.GetActContainer);
                _actButtons.Add(actButton);
            }

            _actButtons[0].Init(_inscriptionsContainer["Buy"], OnBuyClicked);
            _actButtons[1].Init(_inscriptionsContainer["Sell"], OnSellClicked);
            _actButtons[2].Init(_inscriptionsContainer["Speak"], OnSpeakClicked);
            _shopView.GetActExitButton.Init(_inscriptionsContainer["Exit"], OnExitClicked);
        }
        
        public void Dispose()
        {
            Object.Destroy(_shopView.gameObject);
            var transitionScreen = (TransitionPresenter)_screenManager.Open(ScreensEnum.TRANSITION);
            transitionScreen.Hide(() =>
            {
                _gameStateController.CloseDialog();
                _screenManager.Close(ScreensEnum.TRANSITION);
            });
        }
        
        private void Close()
        {
            Dispose();
        }

        private void PlayFailSound()
        {
            
        }
        
        public void PlayBuySound()
        {
            
        }
    }
}