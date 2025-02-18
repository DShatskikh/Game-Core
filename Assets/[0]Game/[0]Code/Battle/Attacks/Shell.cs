using UnityEngine;

namespace Game
{
    public class Shell : MonoBehaviour, IShell
    {
        [SerializeField]
        private bool _isDestroy;

        public bool IsDestroy => _isDestroy;
        
        public void Crash(Heart heart)
        {
            heart.Crash();
            
            if (_isDestroy)
                Destroy(gameObject);
        }
    }
}