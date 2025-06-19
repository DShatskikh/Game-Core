using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    // Визуальная часть окна битвы
    public class BattleView : MonoBehaviour
    {
        [SerializeField]
        private Transform _mainContainer;
        
        [SerializeField]
        private GameObject _turnPanel;
        
        [SerializeField]
        private Transform _attacksContainer;
        
        [SerializeField]
        private Transform _itemsContainer;
        
        [SerializeField]
        private Transform _actionsContainer;
        
        [SerializeField]
        private Transform _mercyContainer;
        
        [SerializeField]
        private Transform _enemiesContainer;
        
        [SerializeField]
        private Transform _actionEnemiesContainer;
        
        [SerializeField]
        private ShopButton _attackButton;
        
        [SerializeField]
        private ShopButton _itemsButton;
        
        [SerializeField]
        private ShopButton _actionsButton;
        
        [SerializeField]
        private ShopButton _mercyButton;
        
        [SerializeField]
        private ShopButton _turnButton;
        
        [SerializeField]
        private Button _rightSelectItemsButton;
        
        [SerializeField]
        private Button _leftSelectItemsButton;

        [SerializeField]
        private TMP_Text _selectPageItemsLabel;

        [SerializeField]
        private TMP_Text _stateLabel;

        [SerializeField]
        private GameObject _info;

        [SerializeField]
        private TMP_Text _infoLabel;

        [SerializeField]
        private ProgressBar _progressBar;
        
        public Transform GetMainContainer => _mainContainer;
        public Transform GetAttacksContainer => _attacksContainer;
        public Transform GetItemsContainer => _itemsContainer;
        public Transform GetActionsContainer => _actionsContainer;
        public Transform GetMercyContainer => _mercyContainer;
        public Transform GetEnemiesContainer => _enemiesContainer;
        public Transform GetActionEnemiesContainer => _actionEnemiesContainer;
        public ShopButton GetAttackButton => _attackButton;
        public ShopButton GetItemsButton => _itemsButton;
        public ShopButton GetActionsButton => _actionsButton;
        public ShopButton GetMercyButton => _mercyButton;
        public ShopButton GetTurnButton => _turnButton;
        public Button GetRightSelectItemsButton => _rightSelectItemsButton;
        public Button GetLeftSelectItemsButton => _leftSelectItemsButton;
        public TMP_Text GetStateLabel => _stateLabel;

        public void ToggleTurnPanel(bool isActive) => 
            _turnPanel.SetActive(isActive);
        
        public void ToggleAttacksContainer(bool isActive) => 
            _attacksContainer.gameObject.SetActive(isActive);

        public void ToggleItemsContainer(bool isActive) => 
            _itemsContainer.gameObject.SetActive(isActive);

        public void ToggleActionsContainer(bool isActive) => 
            _actionsContainer.gameObject.SetActive(isActive);

        public void ToggleMercyContainer(bool isActive) => 
            _mercyContainer.gameObject.SetActive(isActive);
        
        public void ToggleEnemiesContainer(bool isActive) => 
            _enemiesContainer.gameObject.SetActive(isActive);
        
        public void ToggleActionEnemiesContainer(bool isActive) => 
            _actionEnemiesContainer.gameObject.SetActive(isActive);

        public void ToggleStateLabel(bool isActive) => 
            _stateLabel.gameObject.SetActive(isActive);
        
        public void ToggleTurnButton(bool isActive) => 
            _turnButton.gameObject.SetActive(isActive);
        
        public void SetSelectPageText(string text) => 
            _selectPageItemsLabel.text = text;
        
        public void SetStateText(string text) => 
            _stateLabel.text = text;

        public void ToggleInfo(bool isActive) => 
            _info.SetActive(isActive);

        public void SetInfoText(string text) => 
            _infoLabel.text = text;
        
        public void ToggleProgressBar(bool isActive) => 
            _progressBar.gameObject.SetActive(isActive);
    }
}