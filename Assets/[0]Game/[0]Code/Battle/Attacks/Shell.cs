using UnityEngine;

namespace Game
{
    public class Shell : MonoBehaviour, IShell
    {
        [SerializeField]
        private bool _isDestroy;

        public bool IsDestroy => _isDestroy;
        public bool IsAlive { get; set; } = true;

        public void Crash(Heart heart)
        {
            heart.Crash(this);
            
            if (_isDestroy)
                Destroy(gameObject);
        }

        public virtual void Hide() { }
    }
}