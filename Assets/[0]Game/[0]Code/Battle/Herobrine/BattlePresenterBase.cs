using UnityEngine;

namespace Game
{
    public abstract class BattlePresenterBase
    {
        public abstract void Turn();

        public void AddBattleProgress(int i)
        {
            Debug.Log("Add Progress");
        }
    }
}