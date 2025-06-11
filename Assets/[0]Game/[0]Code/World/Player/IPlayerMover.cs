using UnityEngine;

namespace Game
{
    // Интерфейс для движения персонажа
    public interface IPlayerMover
    {
        void Move(Vector2 directionValue, bool isRunValue);
        void Stop();
    }
}