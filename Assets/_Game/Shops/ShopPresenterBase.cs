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
    // Базовый класс магазина
    public abstract class ShopPresenterBase : IDisposable
    {
        private const string MUSIC_EVENT_PARAMETER_PATH = "Игровая ситуация";
        private const int MUSIC_EVENT_INDEX_HASH = 6;
        
        protected private readonly ShopView _shopView;
        protected private readonly GameStateController _gameStateController;
        protected readonly MainInventory _mainInventory;
        private protected readonly WalletService _walletService;
        protected readonly Dictionary<string, string> _inscriptionsContainer;
        private readonly ShopButton _shopButtonPrefab;
        private readonly ScreenManager _screenManager;

        private protected List<ShopButton> _productButtons = new();
        private List<ShopButton> _speakButtons = new();
        private List<ShopButton> _actButtons = new();
        private protected Product[] _products;
        private protected List<SpeakData> _speakData = new();
        
        private readonly float _startMusicParameterIndex;
        private protected readonly IGameRepositoryStorage _mainRepositoryStorage;
        private readonly DiContainer _diContainer;
        private readonly IPurchaseService _purchaseService;

        public ShopPresenterBase(ShopView shopViewPrefab, ShopButton shopButtonPrefab,
            Dictionary<string, string> inscriptionsContainer, GameStateController gameStateController, 
            MainInventory mainInventory, WalletService walletService, 
            DiContainer container, ShopBackground backgroundPrefab, 
            ScreenManager screenManager, IGameRepositoryStorage mainRepositoryStorage, IPurchaseService purchaseService)
        {
            _shopView = Object.Instantiate(shopViewPrefab);
            _inscriptionsContainer = inscriptionsContainer;
            _gameStateController = gameStateController;
            _mainInventory = mainInventory;
            _walletService = walletService;
            _shopButtonPrefab = shopButtonPrefab;
            _screenManager = screenManager;
            _mainRepositoryStorage = mainRepositoryStorage;
            _diContainer = container;
            _purchaseService = purchaseService;

            _gameStateController.OpenCutscene();
            
            RuntimeManager.StudioSystem.getParameterByName(MUSIC_EVENT_PARAMETER_PATH,
                out float startMusicParameterIndex);
            _startMusicParameterIndex = startMusicParameterIndex;
            RuntimeManager.StudioSystem.setParameterByName(MUSIC_EVENT_PARAMETER_PATH, MUSIC_EVENT_INDEX_HASH);

            container.Inject(_shopView.GetMonologueText);

            var background = Object.Instantiate(backgroundPrefab, _shopView.GetBackgroundContainer);
            //container.Inject(background);
        }

        private protected virtual void InitProducts(Product[] products) => 
            _products = products;

        private protected void Init(SpeakData[] speakData, Product[] products)
        {
            InitProducts(products);
            InitSpeaks(speakData);
            CreateSpeakButtons();
            CreateProductButtons();
            CreateActButtons();
            InitItemCountText();
            InitMoneyText();

            OpenStartPanel();
        }

        private void InitSpeaks(SpeakData[] speakData) => 
            _speakData = speakData.ToList();

        protected abstract void Load();
        protected abstract void Save();
        
        private void InitMoneyText()
        {
            _walletService.Money.SubscribeAndCall(money => _shopView.SetMoneyText($"{money}М"));
        }

        private void InitItemCountText()
        {
            _shopView.SetItemCountText($"{_mainInventory.GetItemsCount}/{_mainInventory.MainSlots.Length}");
            _mainInventory.OnSlotChange += (item, slot) => _shopView.SetItemCountText($"{_mainInventory.GetItemsCount}/{_mainInventory.MainSlots.Length}");
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
        
        private protected virtual void OpenStartPanel()
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

        private protected virtual void OnBuyClicked()
        {
            OpenBuyPanel();
        }

        private protected void OpenBuyPanel()
        {
            CloseAllPanel();
            _shopView.ToggleSelectPanel(true);
            _shopView.ToggleStats(true);
            _shopView.ToggleProducts(true);
            _shopView.SetStatsText(_inscriptionsContainer["BuyStats"]);

            EventSystem.current.SetSelectedGameObject(_productButtons[0].gameObject);
        }
        
        protected virtual void CreateProductButtons()
        {
            foreach (var product in _products)
            {
                var productButton = Object.Instantiate(_shopButtonPrefab, _shopView.GetProductContainer);
                _diContainer.Inject(productButton);
                InitProductButton(productButton, product);
                _productButtons.Add(productButton);
            }

            _shopView.GetProductExitButton.onClick.AddListener(OpenStartPanel);
            ((ShopButton)_shopView.GetProductExitButton).OnSelectAction += () => { _shopView.ToggleProductInfo(false); };
        }

        private void AddItemToInventory(Product product)
        {
            if (product.Config.Prototype.TryGetComponent(out AttackComponent attackComponent))
            {
                _mainInventory.EquipWeapon(product.Config.Prototype.Clone());
                return;
            }
            
            if (product.Config.Prototype.TryGetComponent(out ArmorComponent armorComponent))
            {
                _mainInventory.EquipArmor(product.Config.Prototype.Clone());
                return;
            }
            
            _mainInventory.Add(product.Config.Prototype);
        }
        
        private protected void ReplaceProduct(Product current, Product next)
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
#if YandexGamesPlatform_yg
            productButton.GetLabel.text = !product.IsDonation 
                ? $"{product.Price}М - {product.Config.Prototype.MetaData.Name}" 
                : $"{product.DonationPrice}ЯН - {product.Config.Prototype.MetaData.Name}";
#elif RUSTORE
            productButton.GetLabel.text = !product.IsDonation 
                ? $"{product.Price}М - {product.Config.Prototype.MetaData.Name}" 
                : $"{product.DonationPrice}РУБ - {product.Config.Prototype.MetaData.Name}";
#else
            productButton.GetLabel.text = $"{product.Price}М - {product.Config.Prototype.MetaData.Name}";
#endif

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

#if YandexGamesPlatform_yg && RUSTORE
                _purchaseService.BuyPayments(product.Config.Prototype.ID);
#endif
                
                _shopView.GetBuyYesButton.onClick.RemoveAllListeners();
                _shopView.GetBuyYesButton.onClick.AddListener(() => OnBuyYesButton(product, productButton));

                _shopView.GetBuyNoButton.onClick.RemoveAllListeners();
                _shopView.GetBuyNoButton.onClick.AddListener(OnBuyNoButton);
            });

            if (product.Counts == 0)
                ProductButtonInactive(productButton);
        }

        private protected virtual void OnBuyNoButton()
        {
            _shopView.ToggleProductInfo(true);
            _shopView.ToggleBuy(false);
            EventSystem.current.SetSelectedGameObject(_shopView.GetProductExitButton.gameObject);
        }

        private protected virtual void OnBuyYesButton(Product product, ShopButton productButton)
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
        }
        
        protected virtual void BuySuccess(Product product, ShopButton productButton)
        {
            AddItemToInventory(product);
            _shopView.SetStatsText(_inscriptionsContainer["BuySuccess"]);

            if (product.Counts > 0)
                product.Counts -= 1;

            if (product.Counts == 0)
                ProductButtonInactive(productButton);
        }

        private protected virtual void CreateSpeakButtons()
        {
            foreach (var pair in _speakData)
            {
                CreateSpeakButton(pair);
            }

            _shopView.GetSpeakExitButton.onClick.AddListener(SpeakExitButtonClicked);
        }

        private protected virtual void SpeakExitButtonClicked()
        {
            OpenStartPanel();
        }
        
        protected void CreateSpeakButton(SpeakData pair)
        {
            var speakButton = Object.Instantiate(_shopButtonPrefab, _shopView.GetSpeakContainer);
            _diContainer.Inject(speakButton);
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

        private void CreateActButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                var actButton = Object.Instantiate(_shopButtonPrefab, _shopView.GetActContainer);
                _diContainer.Inject(actButton);
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
                _gameStateController.CloseCutscene();
                _screenManager.Close(ScreensEnum.TRANSITION);
                OnClose();
            });
        }

        private protected virtual void OnClose()
        {
            
        }
        
        private protected void Close()
        {
            RuntimeManager.StudioSystem.setParameterByName(MUSIC_EVENT_PARAMETER_PATH, _startMusicParameterIndex);
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