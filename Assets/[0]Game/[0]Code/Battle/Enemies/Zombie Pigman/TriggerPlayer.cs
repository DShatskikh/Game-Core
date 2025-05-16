using UnityEngine;

namespace Game
{
    public sealed class TriggerPlayer : MonoBehaviour
    {
        [SerializeField]
        private TriggerEnter _triggerEnter;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Player>())
            {
                _triggerEnter.SendEventMessage();
            }
        }
    }
}