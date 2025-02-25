using UnityEngine;

namespace Game
{
    public abstract class BattleControllerBase
    {
        public abstract void Turn();

        public void AddBattleProgress(int i)
        {
            Debug.Log("Add Progress");
        }
    }
}