using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class UseObjectAction : MonoBehaviour, IUseObject
    {
        [SerializeField]
        private UnityEvent _event;
        
        public void Use() => 
            _event.Invoke();
    }
}