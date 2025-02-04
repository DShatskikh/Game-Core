using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public sealed class SoundPlayer
    {
        private readonly AudioSource _audioSource_1;
        private readonly AudioSource _audioSource_2;
        
        private bool _isFirst;
        
        public AudioClip Clip => _audioSource_1.clip;
        public static SoundPlayer Instance { get; private set; }

        public SoundPlayer(AudioSource audioSource_1, AudioSource audioSource_2)
        {
            _audioSource_1 = audioSource_1;
            _audioSource_2 = audioSource_2;
            
            Instance = this;
        }

        public static void Play(AudioClip clip)
        {
            Instance.PlayLocal(clip);
        }

        public static void Stop()
        {
            Instance.StopLocal();
        }
        
        public void StopLocal()
        {
            _audioSource_1.Stop();
            _audioSource_2.Stop();
        }

        private void PlayLocal(AudioClip clip)
        {
            if (_isFirst)
            {
                _audioSource_1.clip = clip;
                _audioSource_1.Play();  
            }
            else
            {
                _audioSource_2.clip = clip;
                _audioSource_2.Play();   
            }

            _isFirst = !_isFirst;
        }
    }
}