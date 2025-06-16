using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    // Слайд для заставки
    public sealed class Slide : MonoBehaviour
    {
        [SerializeField]
        private float _delay = 5;

        [SerializeField]
        private UnityEvent SkipAction;
        
        public float GetDelay => _delay;

        public void Skip() => 
            SkipAction.Invoke();
    }
}