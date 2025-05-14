using FMODUnity;
using TMPro;
using UnityEngine;

namespace Game
{
    public sealed class SaveView : ScreenBase
    {
        private const string SOUND_HASH = "event:/Звуки/Сохранились";
        
        [SerializeField]
        private TMP_Text _infoLabel;

        [SerializeField]
        private TMP_Text _savedLabel;
        
        [SerializeField]
        private ShopButton _saveButton;

        [SerializeField]
        private ShopButton _returnButton;
        
        public TMP_Text GetSavedLabel => _savedLabel;
        public TMP_Text GetInfoLabel => _infoLabel;
        public ShopButton GetSaveButton => _saveButton;
        public ShopButton GetReturnButton => _returnButton;
        
        public void SetInfoLabel(string message) => 
            _infoLabel.text = message;

        public void PlaySound()
        {
            RuntimeManager.PlayOneShot(SOUND_HASH);
        }
    }
}