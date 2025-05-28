using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    // Обьект который при взаимодействии вызывает ивент
    public class UseObjectAction : MonoBehaviour, IUseObject
    {
        [SerializeField]
        private UnityEvent _event;

        public UnityEvent GetEvent => _event;
        
        public void Use() => 
            _event.Invoke();
    }
}