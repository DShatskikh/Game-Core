using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMODUnity;
using I2.Loc;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    public abstract class BattleControllerBase : IGameGameOvertListener
    {
        private const string MUSIC_EVENT_PARAMETER_PATH = "Игровая ситуация";
        private const int MUSIC_EVENT_INDEX_HASH = 2;
        
        private readonly BattleView _view;
        private readonly ShopButton _prefabButton;
        private readonly BattlePoints _points;
        private readonly Player _player;
        private readonly Arena _arena;
        private readonly Heart _heart;
        private readonly List<ShopButton> _itemButtons = new();
        private readonly List<ShopButton> _attackButtons = new();
        private readonly List<EnemyBattleButton> _attackEnemyButtons = new();
        private readonly List<EnemyBattleButton> _actionEnemyButtons = new();
        private readonly DiContainer _container;
        private readonly MainInventory _inventory;
        private readonly GameStateController _gameStateController;
        private readonly CinemachineCamera _virtualCamera;
        private readonly TurnProgressStorage _turnProgressStorage;
        private readonly EnemyBattleButton _enemyPrefabButton;
        private readonly TimeBasedTurnBooster _timeBasedTurnBooster;
        private readonly ScreenManager _screenManager;
        private readonly AttackIndicator _attackIndicator;
        private readonly INextButton _nextButton;
        private readonly SerializableDictionary<string, LocalizedString> _localizedPairs;
        private readonly int _damage = 5;
        
        private readonly string _gameOverMessage = "Ты умер от Зомбиии!";
        
        private List<ShopButton> _actionButtons = new();
        private protected int _numberTurn = -1;
        private protected IEnemy[] _enemies;
        private IEnemy _selectedEnemy;
        private ShopButton _mercyButton;
        private protected int _attackIndex;
        private Item _attackItem;
        private readonly bool _isRun = true;
        private float _startMusicParameterIndex;

        public BattleControllerBase(BattleView view, ShopButton prefabButton, MainInventory inventory, 
            GameStateController gameStateController, BattlePoints points, Player player,
            Arena arena, Heart heart, DiContainer container,
            CinemachineCamera virtualCamera, TurnProgressStorage turnProgressStorage, 
            TimeBasedTurnBooster timeBasedTurnBooster, EnemyBattleButton enemyBattleButton, ScreenManager screenManager, 
            AttackIndicator attackIndicator, INextButton nextButton, SerializableDictionary<string, LocalizedString> localizedPairs)
        {
            _view = view;
            _prefabButton = prefabButton;
            _points = points;
            _player = player;
            _arena = arena;
            _heart = heart;
            _container = container;
            _inventory = inventory;
            _gameStateController = gameStateController;
            _virtualCamera = virtualCamera;
            _turnProgressStorage = turnProgressStorage;
            _timeBasedTurnBooster = timeBasedTurnBooster;
            _enemyPrefabButton = enemyBattleButton;
            _screenManager = screenManager;
            _attackIndicator = attackIndicator;
            _nextButton = nextButton;
            _localizedPairs = localizedPairs;
        }

        public abstract void OnGameOver();

        public virtual void Turn()
        {
            CloseAllPanel();
            _view.ToggleTurnPanel(true);
            _view.ToggleStateLabel(true);
            EventSystem.current.SetSelectedGameObject(_view.GetAttackButton.gameObject);
            
            _view.SetStateText(GetStateText());
        }

        private protected abstract IEnemy[] GetAllEnemies();

        private protected virtual void Init()
        {
            _enemies = GetAllEnemies();
            _heart.SetDamage(_damage);
            _heart.OnDeath += Death;

            _gameStateController.StartBattle();
            CloseAllPanel();
            
            _heart.GetHealth.Subscribe(value =>
            {
                if (_numberTurn == -1)
                    return;
                
                if (value <= 0) 
                    _gameStateController.GameOver();
            });

            _numberTurn = 0;

            CreateWeaponSlots(_inventory);
            InitAttackButton();
            InitItemButton();
            InitActionButton();
            InitMercyButton();
            CreateItemButtons(_inventory);
            CreateAttackEnemyButtons();
            CreateActionEnemyButtons();
            InitMercy();
            CloseAllPanel();
            Intro();
        }

        private void CreateWeaponSlots(MainInventory inventory)
        {
            foreach (var attackSlot in inventory.MainSlots)
            {
                CreateWeaponSlot(attackSlot);
            }
        }

        private void CreateActionEnemyButtons()
        {
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

                    CreateActionButtons();

                    EventSystem.current.SetSelectedGameObject(_actionButtons[0].gameObject);
                    //SoundPlayer.Play(AssetProvider.Instance.SelectSound);
                });
            }
        }

        private void CreateActionButtons()
        {
            foreach (var actionButton in _actionButtons)
            {
                Object.Destroy(actionButton.gameObject);
            }

            _actionButtons = new List<ShopButton>();

            foreach (var action in _selectedEnemy.Actions)
            {
                var actionButton = Object.Instantiate(_prefabButton, _view.GetActionsContainer);

                foreach (var monoBehaviour in actionButton.GetComponentsInChildren<MonoBehaviour>())
                {
                    _container.Inject(monoBehaviour);
                }
                
                actionButton.Init(action.Name, () =>
                {
                    CloseAllPanel();
                    _selectedEnemy.Mercy += action.Progress;
                    ActionTurn(_selectedEnemy, action);
                });

                _actionButtons.Add(actionButton);
            }
        }
        
        private void CreateAttackEnemyButtons()
        {
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
        }

        private void CreateItemButtons(MainInventory inventory)
        {
            foreach (var slot in inventory.MainSlots)
            {
                if (!slot.HasItem)
                    continue;

                if (ItemUseCases.TryGetComponent(slot.Item, out AttackComponent attackComponent))
                    continue;

                CreateItemButton(slot);
            }
        }

        private void InitMercyButton()
        {
            _view.GetMercyButton.onClick.AddListener(() =>
            {
                CloseAllPanel();

                _mercyButton.GetLabel.color = Color.white;

                foreach (var enemy in _enemies)
                {
                    if (enemy.CanMercy && !enemy.IsMercy)
                    {
                        _mercyButton.GetLabel.color = Color.yellow;
                        break;
                    }
                }

                _view.ToggleTurnPanel(true);
                _view.ToggleMercyContainer(true);

                EventSystem.current.SetSelectedGameObject(_mercyButton.gameObject);
            });
            
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
        }

        private void InitActionButton()
        {
            _view.GetActionsButton.onClick.AddListener(() =>
            {
                CloseAllPanel();
                
                _view.ToggleTurnPanel(true);
                UpgradeEnemies(_actionEnemyButtons);

                if (_enemies.Length > 1)
                {
                    _view.ToggleActionEnemiesContainer(true); 
                    EventSystem.current.SetSelectedGameObject(GetFirstActiveEnemyButton(_actionEnemyButtons));
                }
                else
                {
                    _selectedEnemy = _enemies[0];
                    _view.ToggleActionsContainer(true);
                    CreateActionButtons();
                    EventSystem.current.SetSelectedGameObject(_actionButtons[0].gameObject);
                }
            });
            
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
        }

        private void InitItemButton()
        {
            _view.GetItemsButton.onClick.AddListener(() =>
            {
                CloseAllPanel();

                _view.ToggleTurnPanel(true);
                _view.ToggleItemsContainer(true);

                EventSystem.current.SetSelectedGameObject(_itemButtons[0].gameObject);
            });
            
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
        }

        private void InitAttackButton()
        {
            _view.GetAttackButton.Init(_localizedPairs["Attack"], () =>
            {
                CloseAllPanel();

                _view.ToggleTurnPanel(true);
                _view.ToggleAttacksContainer(true);

                EventSystem.current.SetSelectedGameObject(_attackButtons[0].gameObject);
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
        }

        private void CreateWeaponSlot(Slot attackSlot)
        {
            if (!attackSlot.HasItem)
                return;

            if (!ItemUseCases.TryGetComponent(attackSlot.Item, out AttackComponent attackComponent))
                return;

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
                    EndFight();
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

        private async UniTask EnemyTurn()
        {
            _heart.transform.position = _arena.transform.position;

            _arena.gameObject.SetActive(true);
            var attackPrefab = GetAttack();
            _heart.gameObject.SetActive(true);
           
            if (attackPrefab)
            {
                await _arena.AwaitSetSize(attackPrefab.GetSizeArena);

                _view.ToggleProgressBar(true);
                _heart.SetProgress(attackPrefab.GetAddProgress);
                _timeBasedTurnBooster.ToggleActivate(true);
                _turnProgressStorage.Reset();
                
                var attack = Object.Instantiate(attackPrefab, _arena.transform.position, Quaternion.identity, _arena.transform);
                _container.Inject(attack);
                _attackIndex++;

                await UniTask.WaitUntil(() => _turnProgressStorage.Progress.Value == 100);
                attack.Hide();
                
                await UniTask.WaitForSeconds(1f);
                Object.Destroy(attack.gameObject);
            }
            else
            {
                
            }

            await _arena.AwaitSetSize(new Vector2(3, 3));
            
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

        private async UniTask EndFight()
        {
            CloseAllPanel();
            _view.ToggleTurnPanel(true);
            _view.ToggleStateLabel(true);
            
            var endText = _localizedPairs["End"].ToString();
            var op = 0;

            foreach (var enemy in _enemies)
            {
                if (enemy.Health < 0) 
                    op += enemy.GetOP;
            }
            
            var money = 0;
            
            foreach (var enemy in _enemies)
            {
                money += enemy.GetMoney;
            }

            string formattedText = string.Format(endText, op, money);
            _view.SetStateText(formattedText);
            
            await _nextButton.WaitShow();

            Exit().Forget();
        }
        
        private string[] GetStartReactions()
        {
            var messages = new List<string>();

            for (int i = 0; i < _enemies.Length; i++)
            {
                messages.Add(_enemies[i].GetStartReaction(i));
            }

            return messages.ToArray();
        }
        
        private async UniTask Intro()
        {
            await BattleIntroUseCases.WaitIntro(_points.GetPartyPositionsData(_player), 
                _points.GetEnemiesPositionsData(_enemies));
            
            RuntimeManager.StudioSystem.getParameterByName(MUSIC_EVENT_PARAMETER_PATH,
                out float startMusicParameterIndex);
            _startMusicParameterIndex = startMusicParameterIndex;
            RuntimeManager.StudioSystem.setParameterByName(MUSIC_EVENT_PARAMETER_PATH, MUSIC_EVENT_INDEX_HASH);
            
            Turn();
            
            await ShowEnemiesReactions(GetStartReactions());
            EventSystem.current.SetSelectedGameObject(_view.GetAttackButton.gameObject);
        }

        private async UniTask ShowEnemyMessage(IEnemy enemy, string message)
        {
            await enemy.MessageBox.AwaitShow(message);
        }

        private async UniTask ShowEnemiesReactions(params string[] messages)
        {
            var messageBoxes = new List<BattleMessageBox>();

            foreach (var enemy in _enemies)
            {
                if (enemy.Health > 0) 
                    messageBoxes.Add(enemy.MessageBox);
            }
            
            await BattleMessageBox.AwaitShow(
                messageBoxes.ToArray(),
                messages
            );
        }

        private string[] GetMercyReactions()
        {
            var messages = new List<string>();
            
            foreach (var enemy in _enemies)
            {
                messages.Add(enemy.GetReaction(enemy.CanMercy ? BattleActionType.Mercy : BattleActionType.NoAction));
            }

            return messages.ToArray();
        }
        
        private async UniTask MercyTurn()
        {
            await ShowEnemiesReactions(GetMercyReactions());
            
            foreach (var enemy in _enemies)
            {
                if (enemy.CanMercy)
                {
                    enemy.IsMercy = true;
                    enemy.Spared();
                    Debug.Log("Пощажен");
                }
            }
            
            if (IsAllDontFight())
            {
                EndFight();
                return;
            }
            
            EnemyTurn();
        }

        private bool IsAllDontFight()
        {
            bool isAllDontFight = true;
                
            foreach (var enemy1 in _enemies)
            {
                if (!enemy1.IsMercy && enemy1.Health > 0)
                {
                    isAllDontFight = false;
                    break;
                }
            }

            return isAllDontFight;
        }

        private string[] GetActionReactions(IEnemy enemy, ActionBattle actionBattle)
        {
            var messages = new List<string>();

            foreach (var enemy1 in _enemies)
            {
                messages.Add(enemy == enemy1 ? enemy1.GetActionReaction(actionBattle) : enemy1.GetReaction(BattleActionType.NoAction));
            }

            return messages.ToArray();
        }
        
        private async UniTask ActionTurn(IEnemy enemy, ActionBattle actionBattle)
        {
            Debug.Log("Добавь текст перед реакцией");
            
            _view.ToggleTurnPanel(true);
            _view.ToggleStateLabel(true);
            _view.SetStateText(actionBattle.Info);
            await _nextButton.WaitShow();
            _view.ToggleTurnPanel(false);
            
            await ShowEnemiesReactions(GetActionReactions(enemy, actionBattle));
            EnemyTurn();
        }

        private string[] GetDeathFriendReactions(IEnemy enemy)
        {
            var messages = new List<string>();

            foreach (var enemy1 in _enemies)
            {
                messages.Add(enemy1.GetDeathFriendReaction(enemy));
            }

            return messages.ToArray();
        }
        
        private string[] GeAttackReactions(IEnemy enemy)
        {
            var messages = new List<string>();

            foreach (var enemy1 in _enemies)
            {
                messages.Add(enemy1.GetReaction(enemy1 == enemy ? BattleActionType.Attack : BattleActionType.NoAction));
            }

            return messages.ToArray();
        }
        
        private string[] GeMissReactions()
        {
            var messages = new List<string>();

            foreach (var enemy1 in _enemies)
            {
                messages.Add("...");
            }

            return messages.ToArray();
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
                await ShowEnemiesReactions(GeMissReactions());
                EnemyTurn();
                return;
            }
            
            enemy.Damage(damage);
            
            if (enemy.Health <= 0)
            {
                await ShowEnemyMessage(enemy, enemy.GetDeathReaction());
                
                await UniTask.WaitForSeconds(0.5f);
                enemy.Death(damage);
                await UniTask.WaitForSeconds(1f);

                if (IsAllDontFight())
                {
                    EndFight();
                    return;
                }

                //Object.Destroy(attackEffect.gameObject);
            }

            await UniTask.WaitForSeconds(1f);
            //Object.Destroy(attackEffect.gameObject);

            if (enemy.Health <= 0)
            {
                await ShowEnemiesReactions(GetDeathFriendReactions(enemy));
            }
            else
            {
                await ShowEnemiesReactions(GeAttackReactions(enemy));
            }
            
            EnemyTurn();
        }
        
        private protected virtual async UniTask Exit()
        {
            foreach (var enemy in _enemies)
            {
                Object.Destroy(((MonoBehaviour)enemy).gameObject);
            }
            
            CloseAllPanel();
            _virtualCamera.gameObject.SetActive(false);

            await UniTask.WaitForSeconds(0.5f);

            Object.Destroy(_view.gameObject);
            _gameStateController.CloseBattle();
            RuntimeManager.StudioSystem.setParameterByName(MUSIC_EVENT_PARAMETER_PATH, _startMusicParameterIndex);
        }
        
        private protected virtual void Death()
        {
            Debug.Log("GameOver");
            Object.Destroy(_view.gameObject);
            _gameStateController.GameOver();

            var gameOverScreen = (GameOverPresenter)_screenManager.Open(ScreensEnum.GAME_OVER);
            gameOverScreen.SetMessage(_gameOverMessage);
        }

        private protected abstract Attack GetAttack();
        private protected abstract string GetStateText();
    }
}