using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public sealed class TriggerAction : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _event;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Player>())
            {
                _event.Invoke();
            }
        }
    }
}