using UnityEngine;

namespace Game
{
    public sealed class MusicPlayer
    {
        private readonly AudioSource _audioSource;
        
        public AudioClip Clip => _audioSource.clip;

        public MusicPlayer(AudioSource audioSource)
        {
            _audioSource = audioSource;
            Instance = this;
        }
        
        public void PlayLocal(AudioClip clip, float time = 0)
        {
            if (_audioSource.clip == clip && _audioSource.isPlaying)
                return;

            _audioSource.clip = clip;

            if (time != 0)
                _audioSource.time = time;
            
            _audioSource.Play();
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public float GetTime() => 
            _audioSource.time;

        public static MusicPlayer Instance { get; private set; }
        
        public static void Play(AudioClip clip, float time = 0)
        {
            Instance.PlayLocal(clip, time);
        }
    }
}