using System.Collections;
using UnityEngine;

namespace Game
{
    // Уничтожается обьект со скриптом через время
    public sealed class TimerDestroy : MonoBehaviour
    {
        [SerializeField]
        private float _duration = 5;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(_duration);
            Destroy(gameObject);
        }
    }
}