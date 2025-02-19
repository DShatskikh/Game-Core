using System.Collections;
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
        
        private GameStateController _gameStateController;
        private DiContainer _diContainer;
        
        [Inject]
        private void Construct(GameStateController gameStateController, DiContainer diContainer, 
            WalletService walletService, CharacterInventory inventory)
        {
            _gameStateController = gameStateController;
            _diContainer = diContainer;
            
            walletService.SetMoney(1250);
            inventory.SetItems(_items);
            inventory.SetWeapons(_weapons);
        }
        
        //ToDo: Код для теста потом перепишу
        public IEnumerator Start()
        {
            yield return null;
            _gameStateController.StartGame();
        }
    }
}