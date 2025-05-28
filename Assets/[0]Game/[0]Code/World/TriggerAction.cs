using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    // Выполняет событие если мы коснулись игрока
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