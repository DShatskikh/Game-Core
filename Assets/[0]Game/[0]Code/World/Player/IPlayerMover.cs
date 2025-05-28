using UnityEngine;

namespace Game
{
    // Интерфейс для движения персонажа
    public interface IPlayerMover
    {
        bool IsMove { get; }
        bool IsRun { get; }
        void Move(Vector2 directionValue, bool isRunValue);
        void Stop();
    }
}