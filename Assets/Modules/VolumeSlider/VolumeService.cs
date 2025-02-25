using UnityEngine;
using UnityEngine.Audio;

namespace Game
{
    public sealed class VolumeService
    {
        private const string VolumeHash = "MasterVolume";
        private int _value;
        private readonly AudioMixer _audioMixer;

        public int GetValue => _value;


        public VolumeService(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;
            
            SetValue(PlayerPrefs.GetInt(VolumeHash, 80));
        }

        public void SetValue(int value)
        {
            _value = value;
            _audioMixer.SetFloat(VolumeHash, Mathf.Lerp(-80, 0, value / 100f));
            
            PlayerPrefs.SetInt(VolumeHash, _value);
        }
    }
}