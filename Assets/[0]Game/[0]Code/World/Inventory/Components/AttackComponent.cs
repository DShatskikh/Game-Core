using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public sealed class AttackComponent : IItemComponent
    {
        public int Attack;
        public GameObject Effect;
        
        public IItemComponent Clone() => 
            new AttackComponent() { Attack = Attack, Effect = Effect };
    }
}