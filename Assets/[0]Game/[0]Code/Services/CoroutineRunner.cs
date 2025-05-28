using UnityEngine;

namespace Game
{
    // Позволяет вызывать корутину из не MonoBehaviour классов
    public sealed class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _coroutineRunner;

        public static CoroutineRunner Instance => _coroutineRunner;

        private void Awake()
        {
            _coroutineRunner = this;
        }
    }
}