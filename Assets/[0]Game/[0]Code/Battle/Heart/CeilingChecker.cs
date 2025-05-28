using UnityEngine;
using Zenject;

namespace Game
{
    // Получаем низ арены
    public sealed class CeilingChecker : MonoBehaviour
    {
        private Arena _arena;
        
        public bool GetIsTouchingCeiling => transform.position.y > _arena.transform.position.y + _arena.SizeField.y / 2;

        [Inject]
        private void Construct(Arena arena)
        {
            _arena = arena;
        }
    }
}