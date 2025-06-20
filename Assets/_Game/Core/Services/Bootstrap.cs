using System.Collections;
using Game.Editor;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Bootstrap : MonoBehaviour
    {
        [Header("Setting")]
        [SerializeField]
        private bool _isTestMode;
        
        [Header("Default Setting")]
        [SerializeField]
        private ItemConfig _defaultWeapon;
        
        [SerializeField]
        private ItemConfig[] _defaultItems;
        
        [LocationID]
        [SerializeField]
        private string _defaultLocationID;

        [SerializeField]
        private int _defaultMoney;

        [SerializeField]
        private ItemConfig _handItem;
        
        [Header("Test")]
        [SerializeField]
        private ItemConfig[] _items;
        
        [SerializeField]
        private ItemConfig _weapon;
        
        [SerializeField]
        private ItemConfig _additionalWeapon;

        [SerializeField]
        private ItemConfig _armor;
        
        [LocationID]
        [SerializeField]
        private string _locationID;

        [SerializeField]
        private int _spawnPointIndex;

        [SerializeField]
        private TutorialStep _tutorialStep;

        [SerializeField]
        private PVPArena.State _pvpState;

        [SerializeField]
        private int _money;
        
        private GameStateController _gameStateController;
        private TutorialState _tutorialState;

        [Inject]
        private void Construct(GameStateController gameStateController, WalletService walletService, 
            MainInventory inventory, LocationsManager locationsManager, IGameRepositoryStorage mainRepositoryStorage,
            TutorialState tutorialState, Player player)
        {
            _gameStateController = gameStateController;
            _tutorialState = tutorialState;

#if UNITY_EDITOR
            
#else
         _isTestMode = false;   
#endif

            inventory.HandSlot.Item = _handItem.Prototype.Clone();
            mainRepositoryStorage.Set(SaveConstants.NOT_FIRST_GAME, new MarkerData());
            
            // если выставленно для тестирования загружаем тестовые данные
            if (_isTestMode)
            {
                mainRepositoryStorage.Set(SaveConstants.TUTORIAL, new TutorialState.Data()
                {
                    CurrentStep = _tutorialStep,
                    IsCompleted = false
                });
            
                mainRepositoryStorage.Set(SaveConstants.PVPARENA, new PVPArena.SaveData()
                {
                    State = _pvpState
                });
            
                walletService.SetMoney(_money);
                inventory.EquipWeapon(_weapon.Prototype.Clone());
                inventory.EquipArmor(_armor.Prototype.Clone());
                inventory.EquipAdditionalWeapon(_additionalWeapon.Prototype.Clone());

                foreach (var item in _items) 
                    inventory.Add(item.Prototype.Clone());

                locationsManager.SwitchLocation(_locationID, _spawnPointIndex);
            }
            // если начали новую игру заружаем дефолтные данные создаем сохранение
            else if (!mainRepositoryStorage.TryGet(SaveConstants.STARTED_GAME, out MarkerData markerData))
            {
                walletService.SetMoney(_defaultMoney);
                inventory.EquipWeapon(_defaultWeapon.Prototype.Clone());

                foreach (var item in _defaultItems) 
                    inventory.Add(item.Prototype.Clone());

                locationsManager.SwitchLocation(_defaultLocationID, 0);
                inventory.Load();
                mainRepositoryStorage.Set(SaveConstants.STARTED_GAME, new MarkerData());
                mainRepositoryStorage.Save();
            }
            // если есть сохранение загружаем игру
            else
            {
                mainRepositoryStorage.TryGet(SaveConstants.LOCATION, out LocationsManager.Data locationData);
                locationsManager.SwitchLocation(locationData.LocationId, locationData.PointIndex);
                player.transform.position = locationData.PlayerPosition;
                inventory.Load();
                
                Debug.Log(locationData);
            }

            _tutorialState.Start();
        }
        
        //ToDo: Код для теста потом перепишу
        public IEnumerator Start()
        {
            yield return null;
            _gameStateController.StartGame();
        }
    }
}