using UnityEngine;
using Zenject;

namespace Game
{
    public abstract class StarterBattleBase : MonoBehaviour
    {
        public abstract void StartBattle();
        protected abstract void Binding(DiContainer subContainer);
    }
}