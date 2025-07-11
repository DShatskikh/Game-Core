﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    // Базовый класс контроллера битвы
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
        private readonly TimeBasedTurnBooster _timeBasedTurnBooster;
        private readonly ScreenManager _screenManager;
        private readonly AttackIndicator _attackIndicator;
        private readonly INextButton _nextButton;
        private readonly SerializableDictionary<string, LocalizedString> _localizedPairs;
        private readonly int _damage = 5;
        
        [Inject]
        private readonly AttackService _attackService;

        private protected abstract string _gameOverMessage { get; }

        private List<ShopButton> _actionButtons = new();
        private protected int _numberTurn = -1;
        private protected IEnemy[] _enemies;
        private protected virtual bool _isRun { get; } = false;
        private IEnemy _selectedEnemy;
        private EnemyBattleButton _enemyPrefabButton;
        private ShopButton _mercyButton;
        private protected int _attackIndex;
        private Item _attackItem;
        private float _startMusicParameterIndex;
        private protected readonly IGameRepositoryStorage _mainRepositoryStorage;
        private readonly HealthService _healthService;
        private readonly LevelService _levelService;
        private readonly WalletService _walletService;
        private readonly HeartModeService _heartModeService;
        private readonly IAssetLoader _assetLoader;

        public BattleControllerBase(BattleView view, ShopButton prefabButton, MainInventory inventory, 
            GameStateController gameStateController, BattlePoints points, Player player,
            Arena arena, Heart heart, DiContainer container,
            CinemachineCamera virtualCamera, TurnProgressStorage turnProgressStorage, 
            TimeBasedTurnBooster timeBasedTurnBooster, EnemyBattleButton enemyBattleButton, ScreenManager screenManager, 
            AttackIndicator attackIndicator, INextButton nextButton, 
            SerializableDictionary<string, LocalizedString> localizedPairs, IGameRepositoryStorage mainRepositoryStorage,
            HealthService healthService, LevelService levelService, WalletService walletService, 
            HeartModeService heartModeService, IAssetLoader assetLoader)
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
            _mainRepositoryStorage = mainRepositoryStorage;
            _healthService = healthService;
            _levelService = levelService;
            _walletService = walletService;
            _heartModeService = heartModeService;
            _assetLoader = assetLoader;
        }

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
            
            _healthService.GetHealth.Subscribe(value =>
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
            CreateAttackEnemyButtons();
            CreateActionEnemyButtons();
            InitMercy();
            CloseAllPanel();
            Intro().Forget();
        }

        private void CreateWeaponSlots(MainInventory inventory)
        {
            if (inventory.WeaponSlot.HasItem)
            {
                CreateWeaponSlot(inventory.WeaponSlot.Item);
            }
            else
            {
                CreateWeaponSlot(inventory.HandSlot.Item);
            }
         
            if (inventory.WeaponAdditionalSlot.HasItem)
                CreateWeaponSlot(inventory.WeaponAdditionalSlot.Item);

            if (!inventory.WeaponSlot.HasItem && inventory.WeaponAdditionalSlot.HasItem)
                _view.GetAttackButton.gameObject.SetActive(false);
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
                    ActionTurn(_selectedEnemy, action).Forget();
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

                    _view.GetTurnButton.onClick.RemoveAllListeners();
                    _view.GetTurnButton.onClick.AddListener(() =>
                    {
                        CloseAllPanel();
                        AttackTurn(enemy, _attackService.GetAttack).Forget();
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
            if (_inventory.GetItemsCount == 0)
            {
                _view.GetItemsButton.interactable = false;
            }
            
            _view.GetItemsButton.onClick.AddListener(() =>
            {
                CloseAllPanel();

                _view.ToggleTurnPanel(true);
                _view.ToggleItemsContainer(true);

                var itemsButtons = _itemButtons.ToList();

                foreach (var itemButton in itemsButtons) 
                    RemoveItemButton(itemButton);

                CreateItemButtons(_inventory);
                
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

        private void CreateWeaponSlot(Item attackItem)
        {
            var attackButton = Object.Instantiate(_prefabButton, _view.GetAttacksContainer);
            _attackButtons.Add(attackButton);
            attackButton.GetLabel.text = attackItem.MetaData.Name;
            attackButton.onClick.AddListener(() =>
            {
                CloseAllPanel();
                UpgradeEnemies(_attackEnemyButtons);

                _view.ToggleTurnPanel(true);
                _view.ToggleEnemiesContainer(true);
                _view.ToggleInfo(false);

                EventSystem.current.SetSelectedGameObject(GetFirstActiveEnemyButton(_attackEnemyButtons).gameObject);

                _attackItem = attackItem;
            });

            attackButton.OnSelectAction += () =>
            {
                _view.ToggleInfo(true);
                _view.SetInfoText($"+{_attackService.GetAttack}УР");
            };
        }

        private GameObject GetFirstActiveEnemyButton(List<EnemyBattleButton> enemyBattleButtons)
        {
            for (int i = 0; i < enemyBattleButtons.Count; i++)
            {
                var enemy = _enemies[i];
                
                if (!enemy.IsDeath && !enemy.IsMercy)
                    return enemyBattleButtons[i].gameObject;
            }

            return null;
        }

        private void UpgradeEnemies(List<EnemyBattleButton> buttons)
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                UpgradeEnemy(buttons[i], _enemies[i]);
            }
        }

        private protected virtual void UpgradeEnemy(EnemyBattleButton enemyButton, IEnemy enemy)
        {
            enemyButton.GetHealthSlider.maxValue = enemy.MaxHealth;
            enemyButton.GetHealthSlider.value = enemy.Health;

            enemyButton.GetMercySlider.value = enemy.Mercy;
            enemyButton.GetMercyLabel.text = $"{enemy.Mercy}%";

            if (enemy.IsDeath)
            {
                enemyButton.GetLabel.color = Color.gray;
                enemyButton.GetLabel.text = enemy.Name + " (Умер)";
                enemyButton.interactable = false;
            }

            if (enemy.IsMercy)
            {
                enemyButton.GetLabel.color = Color.gray;
                enemyButton.GetLabel.text = enemy.Name + " (Пощажен)";
                enemyButton.interactable = false;
            }
        }
        
        private void InitMercy()
        {
            _mercyButton = Object.Instantiate(_prefabButton, _view.GetMercyContainer);
            _mercyButton.Init("Пощада", () =>
            {
                CloseAllPanel();
                MercyTurn().Forget();
            });

            if (_isRun)
            {
                var escapeButton = Object.Instantiate(_prefabButton, _view.GetMercyContainer);
                escapeButton.Init("Сбежать", () =>
                {
                    Debug.Log("Сбежать");
                    EndFight().Forget();
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

        private protected async UniTask EnemyTurn()
        {
            _heart.transform.position = _arena.transform.position;

            _arena.gameObject.SetActive(true);
            var attackPrefab = GetAttack();
            _heart.gameObject.SetActive(true);
           
            if (attackPrefab)
            {
                await _arena.AwaitSetSize(attackPrefab.GetSizeArena);

                _view.ToggleProgressBar(true);
                _heart.SetAddedProgress(attackPrefab.GetShieldAddedProgress);
                _heartModeService.SetMode(attackPrefab.GetStartHeartMode);
                _timeBasedTurnBooster.ToggleActivate(true);
                _timeBasedTurnBooster.SetAddedProgress(attackPrefab.GetTurnAddedProgress);
                _turnProgressStorage.Reset();
                
#if UNITY_WEBGL || UNITY_ANDROID
                GameObject joystick = null;
                GameObject jumpButton = null;
                
                if (DeviceTypeDetector.IsMobile())
                { 
                    joystick = await _assetLoader.InstantiateAsync(AssetPathConstants.JOYSTICK_PATH, _view.GetMainContainer);
                    
                    if (attackPrefab.GetStartHeartMode == Heart.Mode.Blue)
                        jumpButton = await _assetLoader.InstantiateAsync(AssetPathConstants.JUMP_BUTTON_PATH, _view.GetMainContainer);
                }
#endif
                
                var attack = Object.Instantiate(attackPrefab, _view.transform.position, Quaternion.identity, _arena.transform);
                _container.Inject(attack);
                _attackIndex++;

                await UniTask.WaitUntil(() => _turnProgressStorage.Progress.Value == 100);
                attack.Hide();
                
                await UniTask.WaitForSeconds(1f);
                
#if UNITY_WEBGL || UNITY_ANDROID
                if (DeviceTypeDetector.IsMobile())
                {
                    Object.Destroy(joystick);
                    
                    if (jumpButton)
                        Object.Destroy(jumpButton);
                }
#endif
                
                Object.Destroy(attack.gameObject);
            }
            else
            {
                Debug.Log("Нет атаки");
            }

            await _arena.AwaitSetSize(new Vector2(3, 3));
            
            _arena.gameObject.SetActive(false);
            _heart.gameObject.SetActive(false);
            _timeBasedTurnBooster.ToggleActivate(false);
            
            foreach (var enemy in _enemies) 
                enemy.EndEnemyTurn(_numberTurn);

            _numberTurn++;
            Turn();
        }

        private protected void CloseAllPanel()
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
                if (slot.Item.TryGetComponent(out AddMaxHPComponent addMaxHpComponent))
                {
                    Debug.Log($"Увеличи макс хп: {addMaxHpComponent.AddHealth}");
                    _healthService.AddMaxHealth(addMaxHpComponent.AddHealth);
                }
                
                Debug.Log($"Сьели {slot.Item.ID}");
                _healthService.Add(eatComponent.Health);
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

        public async UniTask EndFight()
        {
            CloseAllPanel();
            _view.ToggleTurnPanel(true);
            _view.ToggleStateLabel(true);
            
            var endText = _localizedPairs["End"].ToString();
            var exp = 0;

            foreach (var enemy in _enemies)
            {
                if (enemy.IsDeath) 
                    exp += enemy.GetOP;
            }
            
            var money = 0;
            
            foreach (var enemy in _enemies)
            {
                money += enemy.GetMoney;
            }

            string formattedText = string.Format(endText, exp, money);
            
            _levelService.AddExp(exp, out bool isLevelUp);

            if (isLevelUp)
                formattedText += "\nВы получили новый уровень!";

            _walletService.AddMoney(money);
            _view.SetStateText(formattedText);
            await _nextButton.WaitShow();

            Exit().Forget();
            EndFightAdditional();
        }

        private protected virtual void EndFightAdditional() { }

        private protected virtual void SetEnemyPrefabButton(EnemyBattleButton prefab) => 
            _enemyPrefabButton = prefab;
        
        protected string[] GetStartReactions()
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
            _player.Flip(false);
            
            await BattleIntroUseCases.WaitIntro(_points.GetPartyPositionsData(_player), 
                _points.GetEnemiesPositionsData(_enemies));
            
            RuntimeManager.StudioSystem.getParameterByName(MUSIC_EVENT_PARAMETER_PATH,
                out float startMusicParameterIndex);
            _startMusicParameterIndex = startMusicParameterIndex;
            RuntimeManager.StudioSystem.setParameterByName(MUSIC_EVENT_PARAMETER_PATH, MUSIC_EVENT_INDEX_HASH);

            await StartBattle();
        }

        protected virtual async UniTask StartBattle()
        {
            Turn();
            
            await ShowEnemiesReactions(GetStartReactions());
            EventSystem.current.SetSelectedGameObject(_view.GetAttackButton.gameObject);
        }

        private async UniTask ShowEnemyMessage(IEnemy enemy, string message)
        {
            await enemy.MessageBox.AwaitShow(message);
        }

        protected async UniTask ShowEnemiesReactions(params string[] messages)
        {
            var messageBoxes = new List<BattleMessageBox>();

            foreach (var enemy in _enemies)
            {
                if (!enemy.IsDeath) 
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
                EndFight().Forget();
                return;
            }
            
            EnemyTurn().Forget();
        }

        private bool IsAllDontFight()
        {
            bool isAllDontFight = true;
                
            foreach (var enemy1 in _enemies)
            {
                if (!enemy1.IsMercy && !enemy1.IsDeath)
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
            EnemyTurn().Forget();
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
                messages.Add(enemy1.GetReaction(enemy1 == enemy ? BattleActionType.Attack : BattleActionType.NoAction));

            return messages.ToArray();
        }

        private string[] GeMissReactions()
        {
            var messages = new List<string>();

            foreach (var enemy1 in _enemies) 
                messages.Add(enemy1.GetReaction(BattleActionType.AttackMiss));

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

            if (damage == 0)
            {
                await ShowEnemiesReactions(GeMissReactions());
                EnemyTurn().Forget();
                return;
            }
            
            enemy.Damage(damage);

            _attackItem.TryGetComponent(out AttackComponent attackComponent);
            
            Object.Instantiate(attackComponent.Effect, ((MonoBehaviour)enemy).transform.position.AddY(0.5f), Quaternion.identity);
            
            if (enemy.IsDeath)
            {
                await ShowEnemyMessage(enemy, enemy.GetDeathReaction());
                
                await UniTask.WaitForSeconds(0.5f);
                enemy.Death(damage);
                await UniTask.WaitForSeconds(1f);

                if (IsAllDontFight())
                {
                    EndFight().Forget();
                    return;
                }
            }

            await UniTask.WaitForSeconds(1f);

            if (enemy.IsDeath)
            {
                await ShowEnemiesReactions(GetDeathFriendReactions(enemy));
            }
            else
            {
                await ShowEnemiesReactions(GeAttackReactions(enemy));
            }
            
            EnemyTurn().Forget();
        }

        private protected void SaveDefeat()
        {
            var newDefeatedEnemiesSaveData = new DefeatedEnemiesSaveData();
                
            if (_mainRepositoryStorage.TryGet(SaveConstants.KILLED_ENEMIES, out DefeatedEnemiesSaveData defeatedEnemiesSaveData))
            {
                newDefeatedEnemiesSaveData = defeatedEnemiesSaveData;
            }
                
            newDefeatedEnemiesSaveData.DefeatedEnemies ??= new List<string>();
            newDefeatedEnemiesSaveData.DefeatedEnemies.Add(_enemies[0].GetID);

            Debug.Log($"DefeatedEnemies: {_enemies[0].GetID}");
            
            newDefeatedEnemiesSaveData.KilledEnemies ??= new List<string>();
            
            if (_enemies[0].IsDeath)
            {
                newDefeatedEnemiesSaveData.KilledEnemies.Add(_enemies[0].GetID);
                
                Debug.Log($"KilledEnemies: {_enemies[0].GetID}");
            }
            
            _mainRepositoryStorage.Set(SaveConstants.KILLED_ENEMIES, newDefeatedEnemiesSaveData);
        }
        
        private protected virtual void Death()
        {
            Debug.Log("GameOver");
            Object.Destroy(_view.gameObject);
            _gameStateController.GameOver();

            var gameOverScreen = (GameOverPresenter)_screenManager.Open(ScreensEnum.GAME_OVER);
            gameOverScreen.SetMessage(_gameOverMessage);
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


        private protected abstract Attack GetAttack();
        private protected abstract string GetStateText();
        public abstract void OnGameOver();
    }
}