using System.Collections;
using UnityEngine;

namespace Game
{
    public sealed class MusicClipPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _music;

        private Coroutine _coroutine;

        private void OnEnable()
        {
            if (_coroutine != null) 
                StopCoroutine(_coroutine);
            
            _coroutine = StartCoroutine(AwaitPlay());
        }

        public void Play() => 
            MusicPlayer.Play(_music);

        private IEnumerator AwaitPlay()
        {
            yield return null;
            Play();
        }
    }
}