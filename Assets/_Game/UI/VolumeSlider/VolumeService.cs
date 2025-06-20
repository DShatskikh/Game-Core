using System;
using FMOD.Studio;
using UnityEngine;

namespace Game
{
    // Сервис отвечающий за громкость звука
    public sealed class VolumeService
    {
        private const string BUS_HASH = "bus:/";
        private const string SAVE_KEY = "Volume";
        private Bus _musicBus;
        private float _value;
        private readonly SettingsRepositoryStorage _settingsRepositoryStorage;

        public float GetValue => _value;
        
        // Сохраняемые данные
        [Serializable]
        public struct SaveData
        {
            public float Volume;
        }
        
        public VolumeService(SettingsRepositoryStorage settingsRepositoryStorage)
        {
            Debug.Log("VolumeService");
            
            _settingsRepositoryStorage = settingsRepositoryStorage;
            _settingsRepositoryStorage.Load();
            
            _musicBus = FMODUnity.RuntimeManager.GetBus(BUS_HASH);
            var volume = GetLinearVolume(80f);

            if (_settingsRepositoryStorage.TryGet(SAVE_KEY, out SaveData data))
            {
                volume = data.Volume;
            }
            
            SetValue(volume);
        }

        public void SetValue(float value)
        {
            _value = value;
            _musicBus.setVolume(GetLinearVolume(Mathf.Lerp(-80, 0, value / 100f)));
            
            _settingsRepositoryStorage.Set(SAVE_KEY, new SaveData() { Volume = _value });
            _settingsRepositoryStorage.Save();
        }

        private float GetLinearVolume(float volume)
        {
            return Mathf.Pow(10.0f, volume / 20f);
        }
    }
}