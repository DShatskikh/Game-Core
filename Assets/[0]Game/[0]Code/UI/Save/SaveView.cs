using TMPro;
using UnityEngine;

namespace Game
{
    public sealed class SaveView : ScreenBase
    {
        [SerializeField]
        private TMP_Text _infoLabel;

        [SerializeField]
        private TMP_Text _savedLabel;
        
        [SerializeField]
        private ShopButton _saveButton;

        [SerializeField]
        private ShopButton _returnButton;
        
        public TMP_Text GetSavedLabel => _savedLabel;
        
        public void SetInfoLabel(string message) => 
            _infoLabel.text = message;
    }
}