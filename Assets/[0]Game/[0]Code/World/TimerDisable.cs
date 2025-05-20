using System.Collections;
using UnityEngine;

namespace Game
{
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