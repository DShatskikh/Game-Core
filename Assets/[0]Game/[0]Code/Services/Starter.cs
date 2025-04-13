using System.Collections;
using Game.Editor;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Starter : MonoBehaviour
    {
        [SerializeField]
        private ItemBaseConfig[] _items;
        
        [SerializeField]
        private WeaponItemConfig[] _weapons;

        [LocationID]
        [SerializeField]
        private string _locationID;
        
        private GameStateController _gameStateController;
        
        [Inject]
        private void Construct(GameStateController gameStateController, WalletService walletService, 
            CharacterInventory inventory, LocationsManager locationsManager)
        {
            _gameStateController = gameStateController;
            
            walletService.SetMoney(1250);
            inventory.SetItems(_items);
            inventory.SetWeapons(_weapons);
            
            locationsManager.SwitchLocation(_locationID, 0);
        }
        
        //ToDo: Код для теста потом перепишу
        public IEnumerator Start()
        {
            yield return null;
            _gameStateController.StartGame();
        }
    }
}