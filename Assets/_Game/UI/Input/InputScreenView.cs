using TMPro;
using UnityEngine;

namespace Game
{
    // Визуальная часть основного окна игры
    public class InputScreenView : ScreenBase
    {
        [SerializeField]
        private GameObject _hint;
        
        [SerializeField]
        private TMP_Text _hintLabel;

        [SerializeField]
        private VariableJoystick _joystick;
        
        public void ToggleActivate(bool isActive) => 
            gameObject.SetActive(isActive);
        
        public void ToggleHint(bool isActive) => 
            _hint.SetActive(isActive);
        
        public void SetHintText(string text) => 
            _hintLabel.text = text;
        
        public void ShowJoystick() => 
            _joystick.gameObject.SetActive(true);
    }
}