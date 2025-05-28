using System;
using I2.Loc;
using UnityEngine;

namespace Game
{
    // Логика слайдера
    public sealed class VolumeSliderPresenter : IDisposable
    {
        [SerializeField]
        private LocalizedString _localizedString;

        private readonly VolumeSliderView _view;
        private readonly VolumeService _volumeService;

        public VolumeSliderPresenter(VolumeSliderView view, VolumeService volumeService)
        {
            _view = view;
            _volumeService = volumeService;
            
            view.GetSlider.onValueChanged.AddListener(OnChanged);
            view.GetSlider.maxValue = 100;
            view.GetSlider.value = volumeService.GetValue;
        }

        public void Dispose()
        {
            _view.GetSlider.onValueChanged.RemoveListener(OnChanged);
        }

        private void OnChanged(float value)
        {
            _volumeService.SetValue((int)value);
            _view.SetText($"Звук {(int)value}%");
        }
    }
}