using FMOD.Studio;
using UnityEngine;

namespace Game
{
    public sealed class VolumeService
    {
        private const string BUS_HASH = "bus:/";
        private readonly Bus _musicBus;
        private float _value;
        
        public float GetValue => _value;


        public VolumeService()
        {
            _musicBus = FMODUnity.RuntimeManager.GetBus(BUS_HASH);
            var volume = GetLinearVolume(80f);
            SetValue(PlayerPrefs.GetFloat(BUS_HASH, volume));
        }

        public void SetValue(float value)
        {
            _value = value;
            _musicBus.setVolume(GetLinearVolume(Mathf.Lerp(-80, 0, value / 100f)));
            
            PlayerPrefs.SetFloat(BUS_HASH, _value);
        }

        private float GetLinearVolume(float volume)
        {
            return Mathf.Pow(10.0f, volume / 20f);
        }
    }
}