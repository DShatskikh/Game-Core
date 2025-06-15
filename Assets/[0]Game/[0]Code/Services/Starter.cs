using System.Collections;
using Game.Editor;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Starter : MonoBehaviour
    {
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
        
        private GameStateController _gameStateController;
        private TutorialState _tutorialState;

        [Inject]
        private void Construct(GameStateController gameStateController, WalletService walletService, 
            MainInventory inventory, LocationsManager locationsManager, MainRepositoryStorage mainRepositoryStorage,
            TutorialState tutorialState)
        {
            _gameStateController = gameStateController;
            _tutorialState = tutorialState;
            
            mainRepositoryStorage.Load();
            mainRepositoryStorage.Set(SaveConstants.TUTORIAL, new TutorialState.Data()
            {
                CurrentStep = TutorialStep.BATTLE_BANANA,
                IsCompleted = false
            });
            
            mainRepositoryStorage.Set(SaveConstants.PVPARENA, new PVPArena.SaveData()
            {
                State = PVPArena.State.BANANA
            });
            
            walletService.SetMoney(50);
            inventory.EquipWeapon(_weapon.Prototype.Clone());
            //inventory.EquipArmor(_armor.Prototype.Clone());
            inventory.EquipAdditionalWeapon(_additionalWeapon.Prototype.Clone());

            foreach (var item in _items) 
                inventory.Add(item.Prototype.Clone());

            locationsManager.SwitchLocation(_locationID, _spawnPointIndex);
            
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