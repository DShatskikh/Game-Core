using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    //ToDo: Я еще пишу этот класс, когда доделаю функционал займусь рефакторингом
    public class ShopPresenter : IDisposable
    {
        private readonly ShopView _shopView;
        private readonly GameStateController _gameStateController;
        private readonly ShopButton[] _selectButtons;
        private readonly Product[] _products;
        private readonly TransitionScreen _transitionScreen;
        private readonly CharacterInventory _characterInventory;
        private readonly WalletService _walletService;

        private  List<ShopButton> _productButtons = new();
        private  List<ShopButton> _speakButtons = new();

        public ShopPresenter(ShopView shopView, ShopButton[] selectButtons, ShopButton shopButtonPrefab, 
            Product[] products, GameStateController gameStateController, TransitionScreen transitionScreen, 
            CharacterInventory characterInventory, WalletService walletService)
        {
            _selectButtons = selectButtons;
            _shopView = shopView;
            _gameStateController = gameStateController;
            _products = products;
            _transitionScreen = transitionScreen;
            _characterInventory = characterInventory;
            _walletService = walletService;

            _gameStateController.OpenDialog();

            var speakData = new NameResponsePair[]
            {
                new("Кто ты?", new []{"Я Нотч"}),
                new("О Херобрине", new []{"Он мой брат"}),
            };

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
                });
                
                _speakButtons.Add(speakButton);
            }
            
            _shopView.GetSpeakExitButton.onClick.AddListener(OpenStartPanel);
            
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
                    
                    _shopView.GetBuyYesButton.onClick.RemoveAllListeners();
                    _shopView.GetBuyYesButton.onClick.AddListener(() =>
                    {
                        EventSystem.current.SetSelectedGameObject(_shopView.GetProductExitButton.gameObject);
                        _shopView.ToggleBuy(false);
                        _shopView.ToggleStats(true);

                        if (_walletService.TrySellMoney(product.Price))
                        {
                            _characterInventory.AddItem(product.Config);
                            _shopView.SetStatsText("Спасибо за покупку");
                        }
                        else
                        {
                            _shopView.SetStatsText("Приходи как будут деньги");
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
                
                _productButtons.Add(productButton);
            }
            
            _shopView.GetProductExitButton.onClick.AddListener(OpenStartPanel);
            ((ShopButton)_shopView.GetProductExitButton).OnSelectAction += () =>
            {
                _shopView.ToggleProductInfo(false);
            };

            InitButton(selectButtons[0], "Купить", () =>
            {
                CloseAllPanel();
                _shopView.ToggleSelectPanel(true);
                _shopView.ToggleStats(true);
                _shopView.ToggleProducts(true);
                _shopView.SetStatsText("Чтобы вы хотели купить?");
                
                EventSystem.current.SetSelectedGameObject(_productButtons[0].gameObject);
            });
            
            InitButton(selectButtons[1], "Продать", () =>
            {
                CloseAllPanel();
                _shopView.GetMonologueText.StartWrite(new []{"Это не так работает, я только продаю", "Но ты все еще можешь что-нибудь купить"}, OpenStartPanel);
            });
            
            InitButton(selectButtons[2], "Говорить", () => {
                CloseAllPanel();
                _shopView.ToggleSelectPanel(true);
                _shopView.ToggleSpeakContainer(true);
                _shopView.ToggleStats(true);
                _shopView.SetStatsText("О чем ты хочешь поговорить?");

                EventSystem.current.SetSelectedGameObject(_speakButtons[0].gameObject);
            });
            
            InitButton(selectButtons[3], "Выйти", () =>
            {
                CloseAllPanel();
                _shopView.GetMonologueText.StartWrite(new []{"До встречи"}, Close);
            });

            _shopView.SetItemCountText($"{_characterInventory.Items.Count}/{_characterInventory.MaxCount}");
            _characterInventory.Items.ObserveCountChanged().Subscribe(count => 
                _shopView.SetItemCountText($"{count}/{_characterInventory.MaxCount}"));
            
            _walletService.Money.SubscribeAndCall(money => _shopView.SetMoneyText($"{money}М"));
            
            OpenStartPanel();
        }

        public class ShopPresenterFactory : PlaceholderFactory<ShopPresenter>
        {
            
        }
        
        public void Dispose()
        {
            Object.Destroy(_shopView.gameObject);
            _transitionScreen.Hide(() => _gameStateController.CloseDialog());
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
            _shopView.SetSelectInfoText("Приветик");
            
            EventSystem.current.SetSelectedGameObject(_selectButtons[0].gameObject);
        }

        private void Close() => 
            Dispose();
    }
}