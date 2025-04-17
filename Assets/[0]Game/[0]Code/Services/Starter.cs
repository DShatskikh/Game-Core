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
        private ItemConfig[] _weapons;

        [LocationID]
        [SerializeField]
        private string _locationID;
        
        private GameStateController _gameStateController;
        
        [Inject]
        private void Construct(GameStateController gameStateController, WalletService walletService, 
            MainInventory inventory, LocationsManager locationsManager)
        {
            _gameStateController = gameStateController;
            
            walletService.SetMoney(1250);

            foreach (var item in _items) 
                inventory.Add(item.Prototype.Clone());

            foreach (var item in _weapons) 
                inventory.Add(item.Prototype.Clone());

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