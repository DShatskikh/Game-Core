using UnityEngine;

namespace Game
{
    public abstract class BattleControllerBase : IGameGameOvertListener
    {
        public abstract void Turn();
        public abstract void OnGameOver();
    }
}