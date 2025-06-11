using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game
{
    public sealed class NoSwordTriggerAction : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _actionTrue;
        
        [SerializeField]
        private UnityEvent _actionFalse;

        private MainInventory _inventory;

        [Inject]
        private void Construct(MainInventory inventory)
        {
            _inventory = inventory;
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.GetComponent<Player>())
                return;
            
            if (_inventory.WeaponSlot.HasItem)
            {
                _actionFalse.Invoke();
            }
            else
            {
                _actionTrue.Invoke();
            }
        }
    }
}