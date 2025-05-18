using System;
using FMOD.Studio;
using UnityEngine;

namespace Game
{
    public sealed class VolumeService
    {
        private const string BUS_HASH = "bus:/";
        private const string SAVE_KEY = "Volume";
        private readonly Bus _musicBus;
        private float _value;
        private readonly SettingsRepositoryStorage _settingsRepositoryStorage;

        public float GetValue => _value;
        
        [Serializable]
        public struct Data
        {
            public float Volume;
        }
        
        public VolumeService(SettingsRepositoryStorage settingsRepositoryStorage)
        {
            _settingsRepositoryStorage = settingsRepositoryStorage;
            _settingsRepositoryStorage.Load();
            
            _musicBus = FMODUnity.RuntimeManager.GetBus(BUS_HASH);
            var volume = GetLinearVolume(80f);

            if (_settingsRepositoryStorage.TryGet(SAVE_KEY, out Data data))
            {
                volume = data.Volume;
            }
            
            SetValue(volume);
        }

        public void SetValue(float value)
        {
            _value = value;
            _musicBus.setVolume(GetLinearVolume(Mathf.Lerp(-80, 0, value / 100f)));
            
            _settingsRepositoryStorage.Set(SAVE_KEY, new Data() { Volume = _value });
            _settingsRepositoryStorage.Save();
        }

        private float GetLinearVolume(float volume)
        {
            return Mathf.Pow(10.0f, volume / 20f);
        }
    }
}