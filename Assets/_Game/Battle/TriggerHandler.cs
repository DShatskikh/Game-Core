using System;
using UnityEngine;

namespace Game
{
    public sealed class TriggerHandler : MonoBehaviour
    {
        public event Action TriggerEnter;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Heart>())
            {
                TriggerEnter?.Invoke();
            }
        }
    }
}