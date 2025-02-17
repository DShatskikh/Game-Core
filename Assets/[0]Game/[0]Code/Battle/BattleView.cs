using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BattleView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _turnPanel;
        
        [SerializeField]
        private Transform _attacksContainer;
        
        [SerializeField]
        private Transform _itemsContainer;
        
        [SerializeField]
        private Button _attackButton;
        
        [SerializeField]
        private Button _itemsButton;
        
        [SerializeField]
        private Button _turnButton;
        
        [SerializeField]
        private Button _rightSelectItemsButton;
        
        [SerializeField]
        private Button _leftSelectItemsButton;

        [SerializeField]
        private TMP_Text _selectPageItemsLabel;

        [SerializeField]
        private TMP_Text _stateLabel;
        
        public Transform GetAttacksContainer => _attacksContainer;
        public Transform GetItemsContainer => _itemsContainer;
        public Button GetAttackButton => _attackButton;
        public Button GetItemsButton => _itemsButton;
        public Button GetTurnButton => _turnButton;
        public Button GetRightSelectItemsButton => _rightSelectItemsButton;
        public Button GetLeftSelectItemsButton => _leftSelectItemsButton;
        public TMP_Text GetStateLabel => _stateLabel;
        
        public void ToggleTurnPanel(bool isActive) => 
            _turnPanel.SetActive(isActive);
        
        public void ToggleAttacksContainer(bool isActive) => 
            _attacksContainer.gameObject.SetActive(isActive);
        
        public void ToggleItemsContainer(bool isActive) => 
            _itemsContainer.gameObject.SetActive(isActive);
        
        public void ToggleStateLabel(bool isActive) => 
            _stateLabel.gameObject.SetActive(isActive);
        
        public void ToggleTurnButton(bool isActive) => 
            _turnButton.gameObject.SetActive(isActive);
        
        public void SetSelectPageText(string text) => 
            _selectPageItemsLabel.text = text;
        
        public void SetStateText(string text) => 
            _stateLabel.text = text;
    }
}