using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PixelCrushers.DialogueSystem.Wrappers;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    public sealed class BattleController_Zombie : BattleControllerBase
    {
        private readonly BattleView _view;
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
        private readonly DialogueSystemTrigger _winDialog;
        private readonly CinemachineVirtualCamera _virtualCamera;
        private readonly ProgressStorage _progressStorage;

        private int _numberTurn = -1;

        [Serializable]
        public struct InitData
        {
            public Enemy_Zombie Enemy_Zombie;
        }

        public sealed class Factory : PlaceholderFactory<BattleController_Zombie> { }

        public BattleController_Zombie(BattleView view, ShopButton prefabButton, CharacterInventory inventory, 
            GameStateController gameStateController, InitData initData, BattlePoints points, Player player,
            Arena arena, Heart heart, AudioClip music, DiContainer container, Dictionary<string, string> inscriptionsContainer, 
            DialogueSystemTrigger winDialog, CinemachineVirtualCamera virtualCamera, ProgressStorage progressStorage)
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
            _winDialog = winDialog;
            _virtualCamera = virtualCamera;
            _progressStorage = progressStorage;

            gameStateController.StartBattle();
            CloseAllPanel();

            //_previousMusic = MusicPlayer.Instance.Clip;
           // MusicPlayer.Play(music);

            _heart.GetHealth.Subscribe(value =>
            {
                if (_numberTurn == -1)
                    return;
                
                Debug.Log(value);
                
                if (value <= 0) 
                    _gameStateController.GameOver();
            });

            _numberTurn = 0;
            
            _view.GetAttackButton.Init(_inscriptionsContainer["Attack"], () =>
            {
                CloseAllPanel();
                
                _view.ToggleTurnPanel(true);
                _view.ToggleAttacksContainer(true);
                
                //SoundPlayer.Play(AssetProvider.Instance.SelectSound);
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
                    //SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                    
                    _view.GetTurnButton.onClick.RemoveAllListeners();
                    _view.GetTurnButton.onClick.AddListener(() =>
                    {
                        //SoundPlayer.Play(AssetProvider.Instance.SelectSound);
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

                //SoundPlayer.Play(AssetProvider.Instance.SelectSound);
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
                    
                    //SoundPlayer.Play(AssetProvider.Instance.SelectSound);
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

        public override void Turn()
        {
            CloseAllPanel();
            _view.ToggleTurnPanel(true);
            _view.ToggleStateLabel(true);
            EventSystem.current.SetSelectedGameObject(_view.GetAttackButton.gameObject);
            
            if (_initData.Enemy_Zombie.GetHealth < 15)
                _view.SetStateText("У зомби осталось здоровье на 1 удар");
            else if (_numberTurn == 0)
                _view.SetStateText("Зомби ждет вашего хода");
            else if (_numberTurn == 1)
                _view.SetStateText("Зомби просто стоит и тупит");
            else
                _view.SetStateText("Зомби покорно ждет вашего хода");
        }

        private void Intro()
        {
            _initData.Enemy_Zombie.StartCoroutine(WaitIntro()); //
        }

        private IEnumerator WaitIntro()
        {
            var intro = new BattleIntro(_points.GetPartyPositionsData(_player), _points.GetEnemiesPositionsData(new IEnemy[]{ _initData.Enemy_Zombie }));
            yield return intro.WaitIntro();
            yield return _initData.Enemy_Zombie.GetMessageBox.AwaitShow("(Звуки зомби)");
            Turn();
        }

        private async UniTask EnemyTurn(int damage)
        {
            _player.PlaySwordAttack();
            await UniTask.WaitForSeconds(0.5f);
            var attackEffect = Object.Instantiate(_inventory.Weapons[0].Effect, _initData.Enemy_Zombie.transform.position.AddY(0.5f), Quaternion.identity);

            if (_initData.Enemy_Zombie.GetHealth <= 0)
            {
                _initData.Enemy_Zombie.Death(damage);
                await UniTask.WaitForSeconds(1f);
                Object.Destroy(_initData.Enemy_Zombie.gameObject);
                _virtualCamera.gameObject.SetActive(false);
                await UniTask.WaitForSeconds(1f);
                Exit();
                Object.Destroy(attackEffect.gameObject);
                return;
            }
            else
            {
                _initData.Enemy_Zombie.Damage(damage);
                await UniTask.WaitForSeconds(0.5f);
            }

            await UniTask.WaitForSeconds(1f);
            Object.Destroy(attackEffect.gameObject);

            var enemy = _initData.Enemy_Zombie;
            
            if (_numberTurn == 0)
                await enemy.GetMessageBox.AwaitShow("Р-р-р-р");
            else if (_numberTurn == 1)
                await enemy.GetMessageBox.AwaitShow("(Звуки зомби)");
            else
                await enemy.GetMessageBox.AwaitShow("(Недовольное рычание)");
            
            _heart.transform.position = _arena.transform.position;
            
            _arena.gameObject.SetActive(true);
            _heart.gameObject.SetActive(true);
            
            _progressStorage.Reset();

            await UniTask.WaitForSeconds(1f);
            var attackPrefab = enemy.GetAttacks[0];
            var attack = Object.Instantiate(attackPrefab, _arena.transform);
            _container.Inject(attack);
            
            //await UniTask.WaitForSeconds(10f);
            await UniTask.WaitUntil(() => _progressStorage.Progress.Value == 100);
            attack.Hide();
            
            await UniTask.WaitForSeconds(1f);
            Object.Destroy(attack.gameObject);
            _arena.gameObject.SetActive(false);
            _heart.gameObject.SetActive(false);

            _numberTurn++;
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
            //MusicPlayer.Play(_previousMusic);
            _gameStateController.CloseBattle();
            Object.Destroy(_view.gameObject);
            _winDialog.OnUse();
        }

        public override void OnGameOver()
        {
            Debug.Log("OnGameOver");
            Exit();
        }
    }
}
     