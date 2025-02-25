using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    public class BattleController_Herobrine : BattleControllerBase
    {
        protected readonly BattleView _view;
        private readonly ShopButton _prefabButton;
        private readonly InitData _initData;
        private readonly BattlePoints _points;
        private readonly Player _player;
        private readonly Arena _arena;
        private readonly Heart _heart;
        private readonly AudioClip _previousMusic;
        private readonly List<ShopButton> _itemButtons = new();
        private readonly List<ShopButton> _attackButtons = new();
        private readonly DiContainer _container;
        private readonly CharacterInventory _inventory;
        private readonly GameStateController _gameStateController;
        private readonly Dictionary<string,string> _inscriptionsContainer;

        private int numberTurn;

        [Serializable]
        public struct InitData
        {
            public Enemy_Herobrine EnemyHerobrine;
        }

        public sealed class Factory : PlaceholderFactory<BattleController_Herobrine> { }

        public BattleController_Herobrine(BattleView view, ShopButton prefabButton, CharacterInventory inventory, 
            GameStateController gameStateController, InitData initData, BattlePoints points, Player player,
            Arena arena, Heart heart, AudioClip music, DiContainer container, Dictionary<string, string> inscriptionsContainer)
        {
            _view = view;
            _prefabButton = prefabButton;
            _initData = initData;
            _points = points;
            _player = player;
            _arena = arena;
            _heart = heart;
            _container = container;
            _inventory = inventory;
            _gameStateController = gameStateController;
            _inscriptionsContainer = inscriptionsContainer;

            gameStateController.StartBattle();
            CloseAllPanel();

            _previousMusic = MusicPlayer.Instance.Clip;
            MusicPlayer.Play(music);

            _view.GetAttackButton.Init(_inscriptionsContainer["Attack"], () =>
            {
                CloseAllPanel();
                
                _view.ToggleTurnPanel(true);
                _view.ToggleAttacksContainer(true);
                
                SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                EventSystem.current.SetSelectedGameObject(_attackButtons[0].gameObject);
            });

            _view.GetAttackButton.OnSelectAction += () => _view.ToggleInfo(false);
            
            foreach (var weapon in inventory.Weapons)
            {
                var attackButton = Object.Instantiate(_prefabButton, _view.GetAttacksContainer);
                _attackButtons.Add(attackButton);
                attackButton.GetLabel.text = weapon.GetName;
                attackButton.onClick.AddListener(() =>
                {
                    CloseAllPanel();
                    
                    _view.ToggleTurnPanel(true);
                    _view.ToggleStateLabel(true);
                    _view.ToggleTurnButton(true);
                    _view.SetStateText($"Использовать {weapon}?");
                    _view.ToggleInfo(false);
                    
                    EventSystem.current.SetSelectedGameObject(_view.GetTurnButton.gameObject);
                    SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                    
                    _view.GetTurnButton.onClick.RemoveAllListeners();
                    _view.GetTurnButton.onClick.AddListener(() =>
                    {
                        SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                        _view.ToggleTurnPanel(false);
                        EnemyTurn(weapon.Attack);
                    });
                });
                
                attackButton.OnSelectAction += () =>
                {
                    _view.ToggleInfo(true);
                    _view.SetInfoText(weapon.GetInfo);
                };
            }
            
            _view.GetItemsButton.onClick.AddListener(() =>
            {
                CloseAllPanel();
                
                _view.ToggleTurnPanel(true);
                _view.ToggleItemsContainer(true);

                SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                EventSystem.current.SetSelectedGameObject(_itemButtons[0].gameObject);
            });
            
            _view.GetItemsButton.OnSelectAction += () => _view.ToggleInfo(false);
            
            foreach (var item in inventory.Items)
            {
                var itemButton = Object.Instantiate(_prefabButton, _view.GetItemsContainer);
                _itemButtons.Add(itemButton);
                itemButton.GetLabel.text = item.GetName;
                itemButton.onClick.AddListener(() =>
                {
                    CloseAllPanel();
                    
                    _view.ToggleTurnPanel(true);
                    _view.ToggleStateLabel(true);
                    _view.SetStateText($"Вы использовали {item.GetName}");
                    _view.ToggleInfo(false);
                    _view.GetTurnButton.onClick.RemoveAllListeners();
                    RemoveItem(item, itemButton);
                    
                    SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                    EventSystem.current.SetSelectedGameObject(_view.GetItemsButton.gameObject);
                });
                
                itemButton.OnSelectAction += () =>
                {
                    _view.ToggleInfo(true);
                    _view.SetInfoText(item.GetInfo);
                };
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
            yield return _initData.EnemyHerobrine.GetMessageBox.AwaitShow("Как же ты меня достал");
            Turn();
        }

        private void EnemyTurn(int damage)
        {
            _initData.EnemyHerobrine.StartCoroutine(WaitEnemyTurn(damage)); //
        }

        private IEnumerator WaitEnemyTurn(int damage)
        {
            _initData.EnemyHerobrine.Damage(damage);

            if (_initData.EnemyHerobrine.GetHealth <= 0)
            {
                Object.Destroy(_initData.EnemyHerobrine.gameObject);
                Exit();
                yield break;
            }
            
            yield return new WaitForSeconds(1);
            
            var enemy = _initData.EnemyHerobrine;
            yield return enemy.GetMessageBox.AwaitShow("Готовься");
            
            _arena.gameObject.SetActive(true);
            _heart.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(1);
            var attackPrefab = enemy.GetAttacks[0];
            var attack = Object.Instantiate(attackPrefab, _arena.transform);
            _container.Inject(attack);

            yield return new WaitForSeconds(10);
            attack.Hide();
            
            yield return new WaitForSeconds(1);
            Object.Destroy(attack.gameObject);
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
            EventSystem.current.SetSelectedGameObject(_view.GetAttackButton.gameObject);
            
            if (numberTurn == 0)
                _view.SetStateText("Херобрин ждет вашего хода");
            else
                _view.SetStateText("Херобрин настроен серьезно");
        }

        private void RemoveItem(ItemBaseConfig item, ShopButton button)
        {
            _inventory.RemoveItem(item);
            Debug.Log($"Remove Item {item.GetName}");
            _itemButtons.Remove(button);
            Object.Destroy(button.gameObject);

            if (_itemButtons.Count == 0) 
                _view.GetItemsButton.interactable = false;
        }

        private void Exit()
        {
            Object.Destroy(_view.gameObject);
            MusicPlayer.Play(_previousMusic);
            _gameStateController.CloseBattle();
        }
    }
}