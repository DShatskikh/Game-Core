using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public sealed class GameOverView : ScreenBase
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private TMP_Text _label;

        public Button GetButton =>
            _button;

        public void Show() => 
            gameObject.SetActive(true);

        public void SetLabel(string message) => 
            _label.text = message;
    }
}