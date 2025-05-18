using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class MenuView : ScreenBase
    {
        [SerializeField]
        private ShopButton _continueButton;
        
        [SerializeField]
        private ShopButton _inventoryButton;
        
        [SerializeField]
        private ShopButton _statsButton;
        
        [SerializeField]
        private ShopButton _menuButton;

        [SerializeField]
        private GameObject _statsContainer;
        
        [SerializeField]
        private GameObject _inventoryContainer;

        [SerializeField]
        private Transform _itemsContainer;
        
        [SerializeField]
        private TMP_Text _label;

        [SerializeField]
        private Image _itemIcon;
        
        public ShopButton GetContinueButton => _continueButton;
        public ShopButton GetInventoryButton => _inventoryButton;
        public ShopButton GetStatsButton => _statsButton;
        public ShopButton GetMenuButton => _menuButton;
        public Transform GetItemsContainer => _itemsContainer;

        public void SetLabel(string value) => 
            _label.text = value;

        public void ToggleStats(bool value) => 
            _statsContainer.SetActive(value);

        public void ToggleInventory(bool value) => 
            _inventoryContainer.SetActive(value);
        
        public void SetInventoryIcon(Sprite value) => 
            _itemIcon.sprite = value;
    }
}