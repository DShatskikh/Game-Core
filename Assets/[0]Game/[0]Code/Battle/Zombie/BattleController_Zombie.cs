using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMODUnity;
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
        private const string START_BATTLE_SOUND_PATH = "event:/Звуки/Битва/Начало боя";
        
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
        private readonly List<EnemyBattleButton> _attackEnemyButtons = new();
        private readonly List<EnemyBattleButton> _actionEnemyButtons = new();
        private readonly DiContainer _container;
        private readonly MainInventory _inventory;
        private readonly GameStateController _gameStateController;
        private readonly Dictionary<string,string> _inscriptionsContainer;
        private readonly DialogueSystemTrigger _winDialog;
        private readonly CinemachineVirtualCamera _virtualCamera;
        private readonly TurnProgressStorage _turnProgressStorage;
        private readonly EnemyBattleButton _enemyPrefabButton;
        private readonly TimeBasedTurnBooster _timeBasedTurnBooster;
        private readonly ScreenManager _screenManager;
        private readonly AttackIndicator _attackIndicator;
        private readonly StudioEventEmitter _battleThemeEmitter;
        private readonly bool _isRun = true;
        private readonly string _gameOverMessage = "Ты умер от Зомбиии!";
        private readonly int _damage = 5;

        private int _numberTurn = -1;
        private IEnemy[] _enemies;
        private Item _attackItem;
        private IEnemy _selectedEnemy;
        private List<ShopButton> _actionButtons = new();
        private ShopButton _mercyButton;
        private int _attackIndex;

        [Serializable]
        public struct InitData
        {
            public Enemy_Zombie Enemy_Zombie;
            public Enemy_Zombie Enemy_Zombie_1;
            public Enemy_Zombie Enemy_Zombie_2;
            
            public Attack[] TwoZombieAttacks;
            public Attack[] ThreeZombieAttacks;
        }

        public sealed class Factory : PlaceholderFactory<BattleController_Zombie> { }

        public BattleController_Zombie(BattleView view, ShopButton prefabButton, MainInventory inventory, 
            GameStateController gameStateController, InitData initData, BattlePoints points, Player player,
            Arena arena, Heart heart, StudioEventEmitter battleThemeEmitter, DiContainer container, Dictionary<string, string> inscriptionsContainer, 
            DialogueSystemTrigger winDialog, CinemachineVirtualCamera virtualCamera, TurnProgressStorage turnProgressStorage, 
            TimeBasedTurnBooster timeBasedTurnBooster, EnemyBattleButton enemyBattleButton, ScreenManager screenManager, AttackIndicator attackIndicator)
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
            _turnProgressStorage = turnProgressStorage;
            _timeBasedTurnBooster = timeBasedTurnBooster;
            _enemyPrefabButton = enemyBattleButton;
            _screenManager = screenManager;
            _attackIndicator = attackIndicator;
            _battleThemeEmitter = battleThemeEmitter;

            _heart.SetDamage(_damage);
            _heart.OnDeath += Death;
            
            _enemies = new IEnemy[]
            {
                _initData.Enemy_Zombie,
                _initData.Enemy_Zombie_1,
                _initData.Enemy_Zombie_2
            };
            
            gameStateController.StartBattle();
            CloseAllPanel();
            
            _heart.GetHealth.Subscribe(value =>
            {
                if (_numberTurn == -1)
                    return;
                
                if (value <= 0) 
                    _gameStateController.GameOver();
            });

            _numberTurn = 0;

            foreach (var attackSlot in inventory.MainSlots)
            {
                if (!attackSlot.HasItem)
                    continue;

                if (!ItemUseCases.TryGetComponent(attackSlot.Item, out AttackComponent attackComponent))
                    continue;

                var attackButton = Object.Instantiate(_prefabButton, _view.GetAttacksContainer);
                _attackButtons.Add(attackButton);
                attackButton.GetLabel.text = attackSlot.Item.MetaData.Name;
                attackButton.onClick.AddListener(() =>
                {
                    CloseAllPanel();
                    UpgradeEnemies(_attackEnemyButtons);

                    _view.ToggleTurnPanel(true);
                    _view.ToggleEnemiesContainer(true);
                    _view.ToggleInfo(false);
                    
                    EventSystem.current.SetSelectedGameObject(GetFirstActiveEnemyButton(_attackEnemyButtons).gameObject);
                    
                    _attackItem = attackSlot.Item;
                });
                
                attackButton.OnSelectAction += () =>
                {
                    _view.ToggleInfo(true);
                    _view.SetInfoText(attackSlot.Item.MetaData.Description);
                };
                
                RuntimeManager.PlayOneShot(START_BATTLE_SOUND_PATH);
            }

            _view.GetAttackButton.Init(_inscriptionsContainer["Attack"], () =>
            {
                CloseAllPanel();
                
                _view.ToggleTurnPanel(true);
                _view.ToggleAttacksContainer(true);
                
                EventSystem.current.SetSelectedGameObject(_attackButtons[0].gameObject);
            });

            _view.GetItemsButton.onClick.AddListener(() =>
            {
                CloseAllPanel();

                _view.ToggleTurnPanel(true);
                _view.ToggleItemsContainer(true);

                EventSystem.current.SetSelectedGameObject(_itemButtons[0].gameObject);
            });

            _view.GetActionsButton.onClick.AddListener(() =>
            {
                CloseAllPanel();
                
                _view.ToggleTurnPanel(true);
                UpgradeEnemies(_actionEnemyButtons);
                _view.ToggleActionEnemiesContainer(true);

                
                EventSystem.current.SetSelectedGameObject(GetFirstActiveEnemyButton(_actionEnemyButtons));
            });

            _view.GetMercyButton.onClick.AddListener(() =>
            {
                CloseAllPanel();

                _mercyButton.GetLabel.color = Color.white;
                
                foreach (var enemy in _enemies)
                {
                    if (enemy.Mercy >= 100 && !enemy.IsMercy)
                    {
                        _mercyButton.GetLabel.color = Color.yellow;
                        break;
                    }
                }
                
                _view.ToggleTurnPanel(true);
                _view.ToggleMercyContainer(true);

                EventSystem.current.SetSelectedGameObject(_mercyButton.gameObject);
            });
            
            _view.GetAttackButton.OnSelectAction += () =>
            {
                if (!_view.GetStateLabel.gameObject.activeSelf)
                    _view.SetStateText(GetStateText());

                var isTurnActive = _view.GetTurnButton.gameObject.activeSelf;

                CloseAllPanel();
                
                if (isTurnActive)
                    _view.ToggleTurnButton(true);
                
                _view.ToggleTurnPanel(true);
                _view.ToggleStateLabel(true);
            };

            _view.GetItemsButton.OnSelectAction += () =>
            {
                if (!_view.GetStateLabel.gameObject.activeSelf)
                    _view.SetStateText(GetStateText());
                
                if (!_view.GetStateLabel.gameObject.activeSelf)
                    _view.SetStateText(GetStateText());

                var isTurnActive = _view.GetTurnButton.gameObject.activeSelf;
                
                CloseAllPanel();

                if (isTurnActive)
                    _view.ToggleTurnButton(true);
                
                _view.ToggleTurnPanel(true);
                _view.ToggleStateLabel(true);
            };
            
            _view.GetActionsButton.OnSelectAction += () =>
            {
                if (!_view.GetStateLabel.gameObject.activeSelf)
                    _view.SetStateText(GetStateText());
                
                if (!_view.GetStateLabel.gameObject.activeSelf)
                    _view.SetStateText(GetStateText());

                var isTurnActive = _view.GetTurnButton.gameObject.activeSelf;
                
                CloseAllPanel();
                
                if (isTurnActive)
                    _view.ToggleTurnButton(true);
                
                _view.ToggleTurnPanel(true);
                _view.ToggleStateLabel(true);
            };

            _view.GetMercyButton.OnSelectAction += () =>
            {
                if (!_view.GetStateLabel.gameObject.activeSelf)
                    _view.SetStateText(GetStateText());
                
                if (!_view.GetStateLabel.gameObject.activeSelf)
                    _view.SetStateText(GetStateText());

                var isTurnActive = _view.GetTurnButton.gameObject.activeSelf;
                
                CloseAllPanel();
                
                if (isTurnActive)
                    _view.ToggleTurnButton(true);
                
                _view.ToggleTurnPanel(true);
                _view.ToggleStateLabel(true);
            };


            foreach (var slot in inventory.MainSlots)
            {
                if (!slot.HasItem)
                    continue;

                if (ItemUseCases.TryGetComponent(slot.Item, out AttackComponent attackComponent))
                    continue;
                
                CreateItemButton(slot);
            }
            
            // Выбираем цель для атаки
            foreach (var enemy in _enemies)
            {
                var enemyButton = Object.Instantiate(_enemyPrefabButton, _view.GetEnemiesContainer);
                _attackEnemyButtons.Add(enemyButton);
                enemyButton.GetLabel.text = enemy.Name;
                enemyButton.onClick.AddListener(() =>
                {
                    _selectedEnemy = enemy;
                    CloseAllPanel();
                    
                    _view.ToggleTurnPanel(true);
                    _view.ToggleStateLabel(true);
                    _view.ToggleTurnButton(true);
                    _view.SetStateText($"Использовать {_attackItem.MetaData.Name} на {enemy.Name}?");

                    EventSystem.current.SetSelectedGameObject(_view.GetTurnButton.gameObject);
                    //SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                    
                    _view.GetTurnButton.onClick.RemoveAllListeners();
                    _view.GetTurnButton.onClick.AddListener(() =>
                    {
                        //SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                        CloseAllPanel();
                        _attackItem.TryGetComponent(out AttackComponent attackComponent);
                        AttackTurn(enemy, attackComponent.Attack);
                    });
                });
            }

            // Выбираем цель для действия
            foreach (var enemy in _enemies)
            {
                var enemyButton = Object.Instantiate(_enemyPrefabButton, _view.GetActionEnemiesContainer);
                enemyButton.GetLabel.text = enemy.Name;
                _actionEnemyButtons.Add(enemyButton);
                enemyButton.onClick.AddListener(() =>
                {
                    _selectedEnemy = enemy;
                    CloseAllPanel();
                    
                    // Включаем действия
                    _view.ToggleTurnPanel(true);
                    _view.ToggleActionsContainer(true);

                    foreach (var actionButton in _actionButtons)
                    {
                        Object.Destroy(actionButton.gameObject);
                    }

                    _actionButtons = new List<ShopButton>();
                    
                    foreach (var action in _selectedEnemy.Actions)
                    {
                        var actionButton = Object.Instantiate(_prefabButton, _view.GetActionsContainer);
                        actionButton.Init(action.Name, () =>
                        {
                            CloseAllPanel();
                            _selectedEnemy.Mercy += action.Progress;
                            ActionTurn(enemy, action);
                        });

                        _actionButtons.Add(actionButton);
                    }
                    
                    EventSystem.current.SetSelectedGameObject(_actionButtons[0].gameObject);
                    //SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                });
            }

            InitMercy();
            
            CloseAllPanel();
            Intro();
        }

        private GameObject GetFirstActiveEnemyButton(List<EnemyBattleButton> enemyBattleButtons)
        {
            for (int i = 0; i < enemyBattleButtons.Count; i++)
            {
                var enemy = _enemies[i];
                
                if (enemy.Health > 0 && !enemy.IsMercy)
                    return enemyBattleButtons[i].gameObject;
            }

            return null;
        }

        private void UpgradeEnemies(List<EnemyBattleButton> buttons)
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                var enemyButton = buttons[i];
                enemyButton.GetHealthSlider.maxValue = _enemies[i].MaxHealth;
                enemyButton.GetHealthSlider.value = _enemies[i].Health;

                enemyButton.GetMercySlider.value = _enemies[i].Mercy;
                enemyButton.GetMercyLabel.text = $"{_enemies[i].Mercy}%";

                if (_enemies[i].Health <= 0)
                {
                    enemyButton.GetLabel.color = Color.gray;
                    enemyButton.GetLabel.text = _enemies[i].Name + " (Умер)";
                    enemyButton.interactable = false;
                }

                if (_enemies[i].IsMercy)
                {
                    enemyButton.GetLabel.color = Color.gray;
                    enemyButton.GetLabel.text = _enemies[i].Name + " (Пощажен)";
                    enemyButton.interactable = false;
                }
            }
        }

        private void InitMercy()
        {
            _mercyButton = Object.Instantiate(_prefabButton, _view.GetMercyContainer);
            _mercyButton.Init("Пощада", () =>
            {
                CloseAllPanel();
                MercyTurn();
            });

            if (_isRun)
            {
                var escapeButton = Object.Instantiate(_prefabButton, _view.GetMercyContainer);
                escapeButton.Init("Сбежать", () =>
                {
                    Debug.Log("Сбежать");
                    CloseAllPanel();
                    Exit();
                }); 
            }
        }

        private void CreateItemButton(Slot slot)
        {
            var metadata = slot.Item.MetaData;
            CreateButton(slot, _view.GetItemsContainer, (itemButton) =>
            {
                CloseAllPanel();

                _view.ToggleTurnPanel(true);
                _view.ToggleStateLabel(true);
                _view.SetStateText($"Вы использовали {metadata.Name}");
                _view.ToggleInfo(false);

                _view.GetTurnButton.onClick.RemoveAllListeners();
                UseItem(slot, itemButton);

                //SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                EventSystem.current.SetSelectedGameObject(_view.GetItemsButton.gameObject);
            }, () =>
            {
                _view.ToggleInfo(true);
                _view.SetInfoText(metadata.Description);
            });
        }

        private void CreateButton(Slot slot, Transform container, Action<ShopButton> click, Action select)
        {
            var item = slot.Item;
            var metadata = item.MetaData;
            
            var itemButton = Object.Instantiate(_prefabButton, container);
            _itemButtons.Add(itemButton);
            itemButton.GetLabel.text = metadata.Name;
            itemButton.onClick.AddListener(() => click?.Invoke(itemButton));
            itemButton.OnSelectAction += () => select?.Invoke();
        }

        public override void Turn()
        {
            CloseAllPanel();
            _view.ToggleTurnPanel(true);
            _view.ToggleStateLabel(true);
            EventSystem.current.SetSelectedGameObject(_view.GetAttackButton.gameObject);
            
            _view.SetStateText(GetStateText());
        }

        private string GetStateText()
        {
            if ((_initData.Enemy_Zombie.Mercy >= 100 && !_initData.Enemy_Zombie.IsMercy) 
                || (_initData.Enemy_Zombie_1.Mercy >= 100 && !_initData.Enemy_Zombie_1.IsMercy) 
                || (_initData.Enemy_Zombie_2.Mercy >= 100 && !_initData.Enemy_Zombie_2.IsMercy))
                return "Зомби щадит вас";

            if (_initData.Enemy_Zombie.Health <= 0 || _initData.Enemy_Zombie_1.Health <= 0 || _initData.Enemy_Zombie_2.Health <= 0)
                return "Атмосфера накалилась";
            
            if (_numberTurn == 0)
                return "Зомби ждут вашего хода";
            
            if (_numberTurn == 1)
                return "Зомби просто стоят и тупят";
            
            return "Зомби покорно ждут вашего хода";
        }

        private async UniTask Intro()
        {
            await BattleIntroUseCases.WaitIntro(_points.GetPartyPositionsData(_player), 
                _points.GetEnemiesPositionsData(new IEnemy[]{
                    _initData.Enemy_Zombie_1, _initData.Enemy_Zombie, _initData.Enemy_Zombie_2 }));
            
            Turn();
            
            await ShowEnemiesReactions(
                _enemies[0].GetStartReaction(0),
                _enemies[1].GetStartReaction(1),
                _enemies[2].GetStartReaction(2));
            
            _battleThemeEmitter.Play();
            EventSystem.current.SetSelectedGameObject(_view.GetAttackButton.gameObject);
        }

        private async UniTask ShowEnemyMessage(IEnemy enemy, string message)
        {
            await enemy.MessageBox.AwaitShow(message);
        }

        private async UniTask ShowEnemiesReactions(params string[] messages)
        {
            var messageBoxes = new List<BattleMessageBox>();

            if (_initData.Enemy_Zombie.Health > 0) 
                messageBoxes.Add(_initData.Enemy_Zombie.MessageBox);

            if (_initData.Enemy_Zombie_1.Health > 0) 
                messageBoxes.Add(_initData.Enemy_Zombie_1.MessageBox);
            
            if (_initData.Enemy_Zombie_2.Health > 0) 
                messageBoxes.Add(_initData.Enemy_Zombie_2.MessageBox);
            
            await BattleMessageBox.AwaitShow(
                messageBoxes.ToArray(),
                messages
            );
        }

        private async UniTask MercyTurn()
        {
            Debug.Log("Пощада");
            
            await ShowEnemiesReactions(
                _enemies[0].GetReaction(_enemies[0].Mercy >= 100 ? BattleActionType.Mercy : BattleActionType.NoAction),
                _enemies[1].GetReaction(_enemies[1].Mercy >= 100 ? BattleActionType.Mercy : BattleActionType.NoAction),
                _enemies[2].GetReaction(_enemies[2].Mercy >= 100 ? BattleActionType.Mercy : BattleActionType.NoAction));
            
            foreach (var enemy in _enemies)
            {
                if (enemy.Mercy >= 100)
                {
                    enemy.IsMercy = true;
                    enemy.Spared();
                    Debug.Log("Пощажен");
                }
            }
            
            bool isAllMercy = true;
                
            foreach (var enemy1 in _enemies)
            {
                if (!enemy1.IsMercy)
                {
                    isAllMercy = false;
                    break;
                }
            }

            if (isAllMercy)
            {
                Exit();
                return;
            }
            
            EnemyTurn();
        }

        private async UniTask ActionTurn(IEnemy enemy, ActionBattle actionBattle)
        {
            await ShowEnemiesReactions(
                _enemies[0] == enemy ? _enemies[0].GetActionReaction(actionBattle) : _enemies[0].GetReaction(BattleActionType.NoAction),
                _enemies[1] == enemy ? _enemies[1].GetActionReaction(actionBattle) : _enemies[1].GetReaction(BattleActionType.NoAction),
                _enemies[2] == enemy ? _enemies[2].GetActionReaction(actionBattle) : _enemies[2].GetReaction(BattleActionType.NoAction));

            EnemyTurn();
        }

        private async UniTask AttackTurn(IEnemy enemy, int damage)
        {
            _view.ToggleTurnPanel(true);
            var multiply = await _attackIndicator.GetMultiply();
            damage = (int)(damage * multiply / 100f);
            CloseAllPanel();
            
            _player.PlaySwordAttack();
            await UniTask.WaitForSeconds(0.5f);
            //var attackEffect = Object.Instantiate(_inventory.Weapons[0].Effect, _initData.Enemy_Zombie.transform.position.AddY(0.5f), Quaternion.identity);

            if (damage == 0)
            {
                await ShowEnemiesReactions("...", "...", "...");
                EnemyTurn();
                return;
            }
            
            enemy.Damage(damage);
            
            if (enemy.Health <= 0)
            {
                await ShowEnemyMessage(enemy, enemy.GetDeathReaction());
                
                await UniTask.WaitForSeconds(0.5f);
                enemy.Death(damage);

                bool isAllDeath = true;
                
                foreach (var enemy1 in _enemies)
                {
                    if (enemy1.Health > 0)
                    {
                        isAllDeath = false;
                        break;
                    }
                }

                if (isAllDeath)
                {
                    await UniTask.WaitForSeconds(1f);
                    Object.Destroy(_initData.Enemy_Zombie.gameObject);
                    _virtualCamera.gameObject.SetActive(false);
                    await UniTask.WaitForSeconds(1f);
                    Exit();
                    return;
                }

                //Object.Destroy(attackEffect.gameObject);
            }
            else
            {
                await UniTask.WaitForSeconds(0.5f);
            }

            await UniTask.WaitForSeconds(1f);
            //Object.Destroy(attackEffect.gameObject);

            if (enemy.Health <= 0)
            {
                await ShowEnemiesReactions(
                    _enemies[0].GetDeathFriendReaction(enemy),
                    _enemies[1].GetDeathFriendReaction(enemy),
                    _enemies[2].GetDeathFriendReaction(enemy));
            }
            else
            {
                await ShowEnemiesReactions(
                    _enemies[0].GetReaction(_enemies[0] == enemy ? BattleActionType.Attack : BattleActionType.NoAction),
                    _enemies[1].GetReaction(_enemies[1] == enemy ? BattleActionType.Attack : BattleActionType.NoAction),
                    _enemies[2].GetReaction(_enemies[2] == enemy ? BattleActionType.Attack : BattleActionType.NoAction));
            }
            
            EnemyTurn();
        }

        private Attack GetAttack()
        {
            if (_enemies.Length == 1)
            {
                if (_enemies[0].Attacks.Length >= _attackIndex)
                    _attackIndex = 0;
                
                return _enemies[0].Attacks[_attackIndex];
            }

            if (_enemies.Length == 2)
            {
                if (_initData.TwoZombieAttacks.Length >= _attackIndex)
                    _attackIndex = 0;
                
                return _initData.TwoZombieAttacks[_attackIndex];
            }

            if (_initData.ThreeZombieAttacks.Length >= _attackIndex)
                _attackIndex = 0;
            
            return _initData.ThreeZombieAttacks[_attackIndex];
        }

        private async UniTask EnemyTurn()
        {
            _heart.transform.position = _arena.transform.position;
            
            _arena.gameObject.SetActive(true);
            _heart.gameObject.SetActive(true);
            _view.ToggleProgressBar(true);
            
            _turnProgressStorage.Reset();

            await UniTask.WaitForSeconds(1f);
            var attackPrefab = GetAttack();
            var attack = Object.Instantiate(attackPrefab, _arena.transform);
            _container.Inject(attack);
            _attackIndex++;
            
            _timeBasedTurnBooster.ToggleActivate(true);
            
            await UniTask.WaitUntil(() => _turnProgressStorage.Progress.Value == 100);
            attack.Hide();
            
            await UniTask.WaitForSeconds(1f);
            Object.Destroy(attack.gameObject);
            _arena.gameObject.SetActive(false);
            _heart.gameObject.SetActive(false);
            _timeBasedTurnBooster.ToggleActivate(false);

            _numberTurn++;
            Turn();
        }

        private void CloseAllPanel()
        {
            _view.ToggleAttacksContainer(false);
            _view.ToggleItemsContainer(false);
            _view.ToggleActionsContainer(false);
            _view.ToggleMercyContainer(false);
            _view.ToggleTurnPanel(false);
            _view.ToggleStateLabel(false);
            _view.ToggleTurnButton(false);
            _view.ToggleEnemiesContainer(false);
            _view.ToggleProgressBar(false);
            _view.ToggleInfo(false);
            _view.ToggleActionEnemiesContainer(false);
        }

        private void UseItem(Slot slot, ShopButton button)
        {
            if (slot.Item.TryGetComponent(out EatComponent eatComponent))
            {
                Debug.Log($"Сьели {slot.Item.ID}");
                slot.RemoveItem();
                RemoveItemButton(button);
            }
            else if (slot.Item.TryGetComponent(out ArmorComponent armorComponent))
            {
                Debug.Log($"Одели {slot.Item.ID}");
                _inventory.PutOn(slot);
                RemoveItemButton(button);
                
                if (slot.HasItem)
                    CreateItemButton(slot);
            }

            if (_itemButtons.Count == 0) 
                _view.GetItemsButton.interactable = false;
        }

        private void RemoveItemButton(ShopButton button)
        {
            _itemButtons.Remove(button);
            Object.Destroy(button.gameObject);
        }

        private void Exit()
        {
            _gameStateController.CloseBattle();
            Object.Destroy(_view.gameObject);
            _winDialog.OnUse();
            _battleThemeEmitter.Stop();
        }

        public override void OnGameOver()
        {
            Debug.Log("OnGameOver");
            Exit();
        }

        private void Death()
        {
            Debug.Log("GameOver");
            _battleThemeEmitter.Stop();
            Object.Destroy(_view.gameObject);
            _gameStateController.GameOver();

            var gameOverScreen = (GameOverPresenter)_screenManager.Open(ScreensEnum.GAME_OVER);
            gameOverScreen.SetMessage(_gameOverMessage);
        }
    }
}
     