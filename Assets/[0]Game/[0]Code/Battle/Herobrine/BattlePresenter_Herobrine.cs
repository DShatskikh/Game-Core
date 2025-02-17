using System;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    public class BattlePresenter_Herobrine : BattlePresenter
    {
        protected readonly BattleView _view;
        private readonly ShopButton _prefabButton;
        private readonly InitData _initData;

        [Serializable]
        public struct InitData
        {
            public Enemy_Herobrine EnemyHerobrine;
        }

        public sealed class Factory : PlaceholderFactory<BattlePresenter_Herobrine> { }

        public BattlePresenter_Herobrine(BattleView view, ShopButton prefabButton, CharacterInventory inventory, 
            GameStateController gameStateController, InitData initData)
        {
            _view = view;
            _prefabButton = prefabButton;
            _initData = initData;

            gameStateController.OpenDialog();
            CloseAllPanel();
            
            var attaks = new[]
            {
                "ОБЫЧНАЯ",
                "СИЛЬНАЯ"
            };
            
            _view.GetAttackButton.onClick.AddListener(() =>
            {
                CloseAllPanel();
                
                _view.ToggleTurnPanel(true);
                _view.ToggleAttacksContainer(true);
            });

            foreach (var attack in attaks)
            {
                var attackButton = Object.Instantiate(_prefabButton, _view.GetAttacksContainer);
                attackButton.GetLabel.text = attack;
                attackButton.onClick.AddListener(() =>
                {
                    CloseAllPanel();
                    
                    _view.ToggleTurnPanel(true);
                    _view.ToggleStateLabel(true);
                    _view.ToggleTurnButton(true);
                    _view.SetStateText($"Использовать {attack}?");
                    
                    _view.GetTurnButton.onClick.RemoveAllListeners();
                    _view.GetTurnButton.onClick.AddListener(() =>
                    {
                        _view.ToggleTurnPanel(false);
                        
                        _initData.EnemyHerobrine.Turn(this);
                    });
                });
            }
            
            _view.GetItemsButton.onClick.AddListener(() =>
            {
                CloseAllPanel();
                
                _view.ToggleTurnPanel(true);
                _view.ToggleItemsContainer(true);
            });
            
            foreach (var item in inventory.Items)
            {
                var itemButton = Object.Instantiate(_prefabButton, _view.GetItemsContainer);
                itemButton.GetLabel.text = item.GetName;
                itemButton.onClick.AddListener(() =>
                {
                    CloseAllPanel();
                    
                    _view.ToggleTurnPanel(true);
                    _view.ToggleStateLabel(true);
                    _view.SetStateText($"Вы использовали {item.GetName}");
                    _view.GetTurnButton.onClick.RemoveAllListeners();
                });
            }
            
            Turn();
        }

        private void CloseAllPanel()
        {
            _view.ToggleAttacksContainer(false);
            _view.ToggleItemsContainer(false);
            _view.ToggleTurnPanel(false);
            _view.ToggleStateLabel(false);
            _view.ToggleTurnButton(false);
        }

        public override void Turn()
        {
            CloseAllPanel();
            _view.ToggleTurnPanel(true);
            _view.ToggleStateLabel(true);
            _view.SetStateText("Херобрин ждет вашего хода");
        }
    }
}