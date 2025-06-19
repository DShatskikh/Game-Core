using UnityEngine;

namespace Game
{
    // Базовый класс атаки
    public abstract class Attack : MonoBehaviour
    {
        public abstract Heart.Mode GetStartHeartMode { get; }
        public abstract int GetTurnAddedProgress { get; }
        public abstract int GetShieldAddedProgress { get; }
        public abstract void Hide();
        public virtual Vector2 GetSizeArena => new(3, 3);
    }
}