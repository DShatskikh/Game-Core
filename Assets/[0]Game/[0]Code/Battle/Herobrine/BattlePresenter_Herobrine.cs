using System;
using System.Collections;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    public class BattlePresenter_Herobrine : BattlePresenter
    {
        protected readonly BattleView _view;
        private readonly ShopButton _prefabButton;
        private readonly InitData _initData;
        private readonly BattlePoints _points;
        private readonly Player _player;
        private readonly Arena _arena;
        private readonly Heart _heart;
        
        private int numberTurn;

        [Serializable]
        public struct InitData
        {
            public Enemy_Herobrine EnemyHerobrine;
        }

        public sealed class Factory : PlaceholderFactory<BattlePresenter_Herobrine> { }

        public BattlePresenter_Herobrine(BattleView view, ShopButton prefabButton, CharacterInventory inventory, 
            GameStateController gameStateController, InitData initData, BattlePoints points, Player player,
            Arena arena, Heart heart)
        {
            _view = view;
            _prefabButton = prefabButton;
            _initData = initData;
            _points = points;
            _player = player;
            _arena = arena;
            _heart = heart;

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
                        EnemyTurn();
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
            
            CloseAllPanel();
            Intro();
        }

        private void Intro()
        {
            _initData.EnemyHerobrine.StartCoroutine(WaitIntro()); //
        }

        private IEnumerator WaitIntro()
        {
            var intro = new BattleIntro(_points.GetPartyPositionsData(_player), _points.GetEnemiesPositionsData(new IEnemy[]{ _initData.EnemyHerobrine }));
            yield return intro.WaitIntro();
            yield return _initData.EnemyHerobrine.GetMessageBox.AwaitShow("1... 2... 3...");
            Turn();
        }

        private void EnemyTurn()
        {
            _initData.EnemyHerobrine.StartCoroutine(WaitEnemyTurn()); //
        }

        private IEnumerator WaitEnemyTurn()
        {
            var enemy = _initData.EnemyHerobrine;
            yield return enemy.GetMessageBox.AwaitShow("Готовься");
            
            _arena.gameObject.SetActive(true);
            _heart.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(1);
            var attackPrefab = enemy.GetAttacks[0];
            var attack = Object.Instantiate(attackPrefab);
            
            yield return new WaitForSeconds(3);
            Object.Destroy(attack.gameObject);
            
            yield return new WaitForSeconds(1);
            _arena.gameObject.SetActive(false);
            _heart.gameObject.SetActive(false);

            numberTurn++;
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
            
            if (numberTurn == 0)
                _view.SetStateText("Херобрин ждет вашего хода");
            else
                _view.SetStateText("Херобрин настроен серьезно");
        }
    }
}