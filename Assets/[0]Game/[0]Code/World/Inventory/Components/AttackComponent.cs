using System;
using UnityEngine;

namespace Game
{
    // Компонент предмета который хранит атаку
    [Serializable]
    public sealed class AttackComponent : IItemComponent
    {
        public int Attack;
        public GameObject Effect;
        public Sprite WeaponSprite;

        public IItemComponent Clone() => 
            new AttackComponent() { Attack = Attack, Effect = Effect, WeaponSprite = WeaponSprite };
    }
}