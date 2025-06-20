using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    // Вьюшка слайдера
    public sealed class VolumeSliderView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _label;

        [SerializeField]
        private Slider _slider;

        public Slider GetSlider => _slider;

        public void SetText(string text) => 
            _label.text = text;
    }
}