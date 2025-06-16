using System.Collections;
using UnityEngine;

namespace Game
{
    // Выключает обьект со скриптом через время
    public sealed class TimerDisable : MonoBehaviour
    {
        [SerializeField]
        private float _duration = 5;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(_duration);
            gameObject.SetActive(false);
        }
    }
}