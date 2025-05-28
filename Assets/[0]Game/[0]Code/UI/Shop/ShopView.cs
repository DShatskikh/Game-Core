using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    // Визуальная часть окна магазина
    public class ShopView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private GameObject _selectPanel;

        [SerializeField]
        private TextAnimationWriter _monologueText;
        
        [SerializeField]
        private GameObject _productInfo;

        [SerializeField]
        private TMP_Text _productInfoLabel;

        [SerializeField]
        private Transform _backgroundContainer;
        
        [Header("Left")]
        [SerializeField]
        private TMP_Text _selectInfoLabel;

        [SerializeField]
        private GameObject _products;
        
        [SerializeField]
        private Transform _speakContainer;
        
        [SerializeField]
        private Button _productExitButton;
        
        [SerializeField]
        private Button _speakExitButton;

        [Header("Right")]
        [SerializeField]
        private GameObject _stats;
        
        [SerializeField]
        private TMP_Text _statsLabel;
        
        [SerializeField]
        private GameObject _acts;

        [SerializeField]
        private GameObject _buy;
        
        [SerializeField]
        private TMP_Text _buyLabel;

        [SerializeField]
        private Button _buyYesButton;
        
        [SerializeField]
        private Button _buyNoButton;
        
        [SerializeField]
        private TMP_Text _moneyLabel;
        
        [SerializeField]
        private TMP_Text _itemCountLabel;
        
        [SerializeField]
        private ShopButton _actExitButton;
        
        [SerializeField]
        private Transform _actContainer;
        
        public TextAnimationWriter GetMonologueText => _monologueText;
        public Transform GetSpeakContainer => _speakContainer;
        public Transform GetProductContainer => _products.transform;
        public Button GetProductExitButton => _productExitButton;
        public Button GetSpeakExitButton => _speakExitButton;
        public Button GetBuyYesButton => _buyYesButton;
        public Button GetBuyNoButton => _buyNoButton;
        public ShopButton GetActExitButton => _actExitButton;
        public Transform GetActContainer => _actContainer;
        public Transform GetBackgroundContainer => _backgroundContainer;

        public void ToggleSelectPanel(bool isActive) => 
            _selectPanel.SetActive(isActive);

        public void ToggleMonologueText(bool isActive) => 
            _monologueText.gameObject.SetActive(isActive);
        
        public void SetSelectInfoText(string text) => 
            _selectInfoLabel.text = text;
        
        public void ToggleProducts(bool isActive) => 
            _products.SetActive(isActive);
        
        public void ToggleStats(bool isActive) => 
            _stats.SetActive(isActive);
        
        public void ToggleActs(bool isActive) => 
            _acts.SetActive(isActive);
        
        public void ToggleSelectInfoText(bool isActive) => 
            _selectInfoLabel.gameObject.SetActive(isActive);
        
        public void ToggleSpeakContainer(bool isActive) => 
           _speakContainer.gameObject.SetActive(isActive);
        
        public void ToggleProductInfo(bool isActive) => 
            _productInfo.SetActive(isActive);
        
        public void SetProductInfoText(string text) => 
            _productInfoLabel.text = text;
        
        public void SetStatsText(string text) => 
            _statsLabel.text = text;
        
        public void ToggleBuy(bool isActive) => 
            _buy.SetActive(isActive);
        
        public void SetBuyText(string text) => 
            _buyLabel.text = text;

        public void SetMoneyText(string text) => 
            _moneyLabel.text = text;

        public void SetItemCountText(string text) => 
            _itemCountLabel.text = text;
    }
}