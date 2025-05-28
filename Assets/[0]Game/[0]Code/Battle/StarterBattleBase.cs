using UnityEngine;
using Zenject;

namespace Game
{
    // Базовый класс начала битвы для вызова начала битвы из других классов
    public abstract class StarterBattleBase : MonoBehaviour
    {
        public abstract void StartBattle();
        protected abstract void Binding(DiContainer subContainer);
    }
}