using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game
{
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
            
            heartModeService.Upgrade += Upgrade;
        }

        private void OnDestroy()
        {
            _heartModeService.Upgrade -= Upgrade;
        }

        private void Upgrade(Heart.Mode mode)
        {
            _icon.sprite = _heartModeService.GetIcon();
        }
    }
}