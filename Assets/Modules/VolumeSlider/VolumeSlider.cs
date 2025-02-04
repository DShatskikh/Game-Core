using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    public sealed class VolumeSlider : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _label;

        [SerializeField]
        private LocalizedString _localizedString;
        
        private Slider _slider;
        private VolumeService _volumeService;

        [Inject]
        private void Construct(VolumeService volumeService)
        {
            _volumeService = volumeService;
            _slider = GetComponent<Slider>();
        }

        private void OnEnable()
        {
            _slider.onValueChanged.AddListener(OnChanged);
            //_slider.value = _volumeService.Volume.Value;
            
            LocalizationSettings.SelectedLocaleChanged += LocalizationSettingsOnSelectedLocaleChanged;
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(OnChanged);
            
            LocalizationSettings.SelectedLocaleChanged -= LocalizationSettingsOnSelectedLocaleChanged;
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