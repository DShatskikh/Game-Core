using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    // Иконка сердца
    public sealed class HeartIcon : MonoBehaviour
    {
        private Image _icon;
        private HeartModeService _heartModeService;

        [Inject]
        private void Construct(HeartModeService heartModeService)
        {
            _icon = GetComponent<Image>();
            _heartModeService = heartModeService;
            _icon.sprite = heartModeService.GetIcon();
            
            _heartModeService.Upgrade += Upgrade;
        }

        private void OnDestroy()
        {
            if (_heartModeService != null)
                _heartModeService.Upgrade -= Upgrade;
        }

        private void Upgrade(Heart.Mode mode) => 
            _icon.sprite = _heartModeService.GetIcon();
    }
}