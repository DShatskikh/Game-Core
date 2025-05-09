﻿using UnityEngine;

namespace Game
{
    public abstract class Attack : MonoBehaviour
    {
        public abstract void Hide();
        public virtual Vector2 GetSizeArena => new(3, 3);
        public virtual int GetAddProgress => 3;
    }
}