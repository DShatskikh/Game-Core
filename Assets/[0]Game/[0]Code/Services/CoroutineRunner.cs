using System.Collections;

namespace Game
{
    public sealed class CoroutineRunner
    {
        private static CoroutineRunner _coroutineRunner;

        public static CoroutineRunner Instance => _coroutineRunner;

        public void StartCoroutine(IEnumerator awaitTransition)
        {
            throw new System.NotImplementedException();
        }
    }
}