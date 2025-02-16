using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UniRx;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    public abstract class ShopPresenterBase : IDisposable
    {
        protected readonly ShopView _shopView;
        private readonly GameStateController _gameStateController;
        private readonly TransitionScreen _transitionScreen;
        protected readonly CharacterInventory _characterInventory;
        private readonly WalletService _walletService;
        protected readonly Dictionary<string, string> _inscriptionsContainer;
        private readonly AudioClip _currentMusic;
        private readonly ShopButton _shopButtonPrefab;

        protected List<ShopButton> _productButtons = new();
        private List<ShopButton> _speakButtons = new();
        private List<ShopButton> _actButtons = new();
        protected Product[] _products;
        protected List<SpeakData> _speakData = new();

        public ShopPresenterBase(ShopView shopViewPrefab, ShopButton shopButtonPrefab,
            Dictionary<string, string> inscriptionsContainer, GameStateController gameStateController, 
            TransitionScreen transitionScreen, CharacterInventory characterInventory, WalletService walletService, 
            DiContainer container, AudioClip music, ShopBackground background)
        {
            _shopView = Object.Instantiate(shopViewPrefab);
            _inscriptionsContainer = inscriptionsContainer;
            _gameStateController = gameStateController;
            _transitionScreen = transitionScreen;
            _characterInventory = characterInventory;
            _walletService = walletService;
            _shopButtonPrefab = shopButtonPrefab;

            _gameStateController.OpenDialog();
            _currentMusic = MusicPlayer.Instance.Clip;
            MusicPlayer.Play(music);

            container.Inject(_shopView.GetMonologueText);

            Object.Instantiate(background, _shopView.GetBackgroundContainer);
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
            _shopView.SetItemCountText($"{_characterInventory.Items.Count}/{_characterInventory.MaxCount}");
            _characterInventory.Items.ObserveCountChanged().Subscribe(count =>
                _shopView.SetItemCountText($"{count}/{_characterInventory.MaxCount}"));
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
            SoundPlayer.Play(AssetProvider.Instance.SelectSound);
            _shopView.GetMonologueText.StartWrite(new[] { _inscriptionsContainer["Farewell"] }, Close);
            MusicPlayer.Play(_currentMusic);
            Save();
        }

        private void OnSpeakClicked()
        {
            CloseAllPanel();
            _shopView.ToggleSelectPanel(true);
            _shopView.ToggleSpeakContainer(true);
            _shopView.ToggleStats(true);
            _shopView.SetStatsText(_inscriptionsContainer["SpeakStats"]);
            
            SoundPlayer.Play(AssetProvider.Instance.SelectSound);
            EventSystem.current.SetSelectedGameObject(_speakButtons[0].gameObject);
        }

        private void OnSellClicked()
        {
            CloseAllPanel();
            _shopView.GetMonologueText.StartWrite(
                new[] { _inscriptionsContainer["SellNotCan"] }, OpenStartPanel);
            
            SoundPlayer.Play(AssetProvider.Instance.SelectSound);
        }

        private void OnBuyClicked()
        {
            CloseAllPanel();
            _shopView.ToggleSelectPanel(true);
            _shopView.ToggleStats(true);
            _shopView.ToggleProducts(true);
            _shopView.SetStatsText(_inscriptionsContainer["BuyStats"]);

            SoundPlayer.Play(AssetProvider.Instance.SelectSound);
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
                SoundPlayer.Play(AssetProvider.Instance.SelectSound);
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
            productButton.GetLabel.text = $"{product.Price}М - {product.Config.GetName}";
            productButton.OnSelectAction += () =>
            {
                _shopView.ToggleProductInfo(true);
                _shopView.SetProductInfoText(product.Config.GetInfo);
            };

            productButton.onClick.AddListener(() =>
            {
                _shopView.ToggleProductInfo(false);
                _shopView.ToggleStats(false);
                _shopView.ToggleBuy(true);
                _shopView.SetBuyText($"Купить за {product.Price}М");
                EventSystem.current.SetSelectedGameObject(_shopView.GetBuyYesButton.gameObject);
                SoundPlayer.Play(AssetProvider.Instance.SelectSound);

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
                        SoundPlayer.Play(AssetProvider.Instance.FailSound);
                    }
                });

                _shopView.GetBuyNoButton.onClick.RemoveAllListeners();
                _shopView.GetBuyNoButton.onClick.AddListener(() =>
                {
                    _shopView.ToggleProductInfo(true);
                    _shopView.ToggleBuy(false);
                    SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                    EventSystem.current.SetSelectedGameObject(_shopView.GetProductExitButton.gameObject);
                });
            });

            if (product.Counts == 0)
                ProductButtonInactive(productButton);
        }

        protected virtual void BuySuccess(Product product, ShopButton productButton)
        {
            _characterInventory.AddItem(product.Config);
            _shopView.SetStatsText(_inscriptionsContainer["BuySuccess"]);
            SoundPlayer.Play(AssetProvider.Instance.BuySound);

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
                SoundPlayer.Play(AssetProvider.Instance.SelectSound);
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
                SoundPlayer.Play(AssetProvider.Instance.SelectSound);
            });
        }

        protected void InitActButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                var actButton = Object.Instantiate(_shopButtonPrefab, _shopView.GetActContainer);
                _actButtons.Add(actButton);
            }

            InitButton(_actButtons[0],  _inscriptionsContainer["Buy"], OnBuyClicked);
            InitButton(_actButtons[1], _inscriptionsContainer["Sell"], OnSellClicked);
            InitButton(_actButtons[2], _inscriptionsContainer["Speak"], OnSpeakClicked);
            InitButton((ShopButton)_shopView.GetActExitButton, _inscriptionsContainer["Exit"], OnExitClicked);
        }
        
        private void InitButton(ShopButton shopButton, string text, UnityAction action)
        {
            shopButton.GetLabel.text = text;
            shopButton.onClick.AddListener(action);
        }
        
        public void Dispose()
        {
            Object.Destroy(_shopView.gameObject);
            _transitionScreen.Hide(() => _gameStateController.CloseDialog());
        }
        
        private void Close() => 
            Dispose();
    }
}