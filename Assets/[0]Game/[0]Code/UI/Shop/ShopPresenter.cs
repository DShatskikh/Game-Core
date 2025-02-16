using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    //ToDo: Я еще пишу этот класс, когда доделаю функционал займусь рефакторингом
    public class ShopPresenter : IDisposable
    {
        private readonly ShopView _shopView;
        private readonly GameStateController _gameStateController;
        private readonly TransitionScreen _transitionScreen;
        private readonly CharacterInventory _characterInventory;
        private readonly WalletService _walletService;
        private readonly Dictionary<string, string> _inscriptionsContainer;
        private readonly AudioClip _currentMusic;

        private List<ShopButton> _productButtons = new();
        private List<ShopButton> _speakButtons = new();
        private List<ShopButton> _actButtons = new();
        private readonly InitData _data;

        [Serializable]
        public struct InitData
        {
            public Product[] Products;
            public NameResponsePair[] SpeakData;
        }

        [Serializable]
        public struct ProductSaveData
        {
            public string Id;
            public int Counts;
        }

        [Serializable]
        public struct SaveData
        {
            public List<ProductSaveData> ProductSavesData;
        }

        public class ShopPresenterFactory : PlaceholderFactory<ShopPresenter> { }

        public ShopPresenter(ShopView shopViewPrefab, ShopButton shopButtonPrefab, InitData data,
            Dictionary<string, string> inscriptionsContainer, GameStateController gameStateController, 
            TransitionScreen transitionScreen, CharacterInventory characterInventory, WalletService walletService, 
            DiContainer container, AudioClip music)
        {
            _shopView = Object.Instantiate(shopViewPrefab);
            _inscriptionsContainer = inscriptionsContainer;
            _gameStateController = gameStateController;
            _transitionScreen = transitionScreen;
            _characterInventory = characterInventory;
            _walletService = walletService;
            _data = data;

            _gameStateController.OpenDialog();
            _currentMusic = MusicPlayer.Instance.Clip;
            MusicPlayer.Play(music);

            container.Inject(_shopView.GetMonologueText);

            Load(data);
            InitSpeakButtons(shopButtonPrefab, data.SpeakData);
            InitProductButtons(shopButtonPrefab, data.Products);
            InitActButtons(shopButtonPrefab);
            InitItemCountText();
            InitMoneyText();
            
            OpenStartPanel();
        }

        public void Dispose()
        {
            Object.Destroy(_shopView.gameObject);
            _transitionScreen.Hide(() => _gameStateController.CloseDialog());
        }

        private static void Load(InitData data)
        {
            if (!RepositoryStorage.TryGet(nameof(SaveData), out SaveData saveData))
                return;
            
            foreach (var product in data.Products)
            {
                foreach (var productSaveData in saveData.ProductSavesData)
                {
                    if (product.Config.GetName == productSaveData.Id)
                        product.Counts = productSaveData.Counts;
                }
            }
        }

        private void Save()
        {
            var saveData = new SaveData
            {
                ProductSavesData = new List<ProductSaveData>()
            };

            foreach (var productData in _data.Products)
            {
                saveData.ProductSavesData.Add(new ProductSaveData() {Id = productData.Config.GetName,
                    Counts = productData.Counts});
            }
            
            RepositoryStorage.Set(nameof(SaveData), saveData);
        }

        private void InitMoneyText()
        {
            _walletService.Money.SubscribeAndCall(money => _shopView.SetMoneyText($"{money}М"));
        }

        private void InitItemCountText()
        {
            _shopView.SetItemCountText($"{_characterInventory.Items.Count}/{_characterInventory.MaxCount}");
            _characterInventory.Items.ObserveCountChanged().Subscribe(count =>
                _shopView.SetItemCountText($"{count}/{_characterInventory.MaxCount}"));
        }

        private void InitActButtons(ShopButton prefab)
        {
            for (int i = 0; i < 3; i++)
            {
                var actButton = Object.Instantiate(prefab, _shopView.GetActContainer);
                _actButtons.Add(actButton);
            }

            InitButton(_actButtons[0],  _inscriptionsContainer["Buy"], OnBuyClicked);
            InitButton(_actButtons[1], _inscriptionsContainer["Sell"], OnSellClicked);
            InitButton(_actButtons[2], _inscriptionsContainer["Speak"], OnSpeakClicked);
            InitButton((ShopButton)_shopView.GetActExitButton, _inscriptionsContainer["Exit"], OnExitClicked);
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

        private void InitProductButtons(ShopButton shopButtonPrefab, Product[] products)
        {
            foreach (var product in products)
            {
                var productButton = Object.Instantiate(shopButtonPrefab, _shopView.GetProductContainer);
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
                            _characterInventory.AddItem(product.Config);
                            _shopView.SetStatsText(_inscriptionsContainer["BuySuccess"]);
                            SoundPlayer.Play(AssetProvider.Instance.BuySound);

                            if (product.Counts > 0) 
                                product.Counts -= 1;

                            if (product.Counts == 0) 
                                ProductButtonInactive(productButton);
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
                
                _productButtons.Add(productButton);
            }

            _shopView.GetProductExitButton.onClick.AddListener(() =>
            {
                OpenStartPanel();
                SoundPlayer.Play(AssetProvider.Instance.SelectSound);
            });
            
            ((ShopButton)_shopView.GetProductExitButton).OnSelectAction += () => { _shopView.ToggleProductInfo(false); };
        }

        private static void ProductButtonInactive(ShopButton productButton)
        {
            productButton.GetLabel.text = "--Продано--";
            productButton.GetLabel.color = new Color(1, 1, 1, 0.5f);
            productButton.interactable = false;
        }

        private void InitSpeakButtons(ShopButton shopButtonPrefab, NameResponsePair[] speakData)
        {
            foreach (var pair in speakData)
            {
                var speakButton = Object.Instantiate(shopButtonPrefab, _shopView.GetSpeakContainer);
                speakButton.GetLabel.text = pair.Name;
                speakButton.onClick.AddListener(() =>
                {
                    _shopView.GetMonologueText.StartWrite(pair.Responces, OpenStartPanel);
                    _shopView.ToggleSpeakContainer(false);
                    _shopView.ToggleStats(false);
                    _shopView.ToggleSelectPanel(false);
                    SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                });

                _speakButtons.Add(speakButton);
            }

            _shopView.GetSpeakExitButton.onClick.AddListener(() =>
            {
                OpenStartPanel();
                SoundPlayer.Play(AssetProvider.Instance.SelectSound);
            });
        }

        private void InitButton(ShopButton shopButton, string text, UnityAction action)
        {
            shopButton.GetLabel.text = text;
            shopButton.onClick.AddListener(action);
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

        private void OpenStartPanel()
        {
            CloseAllPanel();
            _shopView.ToggleSelectPanel(true);
            _shopView.ToggleActs(true);
            _shopView.ToggleSelectInfoText(true);
            _shopView.SetSelectInfoText(_inscriptionsContainer["Welcome"]);
            
            EventSystem.current.SetSelectedGameObject(_actButtons[0].gameObject);
        }

        private void Close() => 
            Dispose();
    }
}