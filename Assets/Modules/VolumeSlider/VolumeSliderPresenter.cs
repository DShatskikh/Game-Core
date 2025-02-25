using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Game
{
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
            //_slider.value = _volumeService.Volume.Value;
            
            //LocalizationSettings.SelectedLocaleChanged += LocalizationSettingsOnSelectedLocaleChanged;

            view.GetSlider.maxValue = 100;
            view.GetSlider.value = volumeService.GetValue;
        }

        public void Dispose()
        {
            _view.GetSlider.onValueChanged.RemoveListener(OnChanged);
            
            //LocalizationSettings.SelectedLocaleChanged -= LocalizationSettingsOnSelectedLocaleChanged;
            
            ((IDisposable)_localizedString)?.Dispose();
        }

        private void Start()
        {
            //_slider.value = _volumeService.Volume.Value;
            
            /*LocalizedTextUtility.Load(_localizedString, loadText =>
            {
                _label.text = $"{loadText} {(int)(_slider.value * 100)}%";
            });*/
        }

        private void OnChanged(float value)
        {
            _volumeService.SetValue((int)value);
            _view.SetText($"Звук {(int)value}%");
            
            //_volumeService.Volume.Value = value;
            /*LocalizedTextUtility.Load(_localizedString, loadText =>
            {
                _label.text = $"{loadText} {(int)(value * 100)}%";
            });*/
            
            //RepositoryStorage.Set(KeyConstants.Volume, new VolumeData() { Volume = value });
        }

        private void LocalizationSettingsOnSelectedLocaleChanged(Locale obj)
        {
            //OnChanged(_volumeService.Volume.Value);
        }
    }
}