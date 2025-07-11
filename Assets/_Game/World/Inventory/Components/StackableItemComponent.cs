﻿using System;

namespace Game
{
    // Компонент предмета который позволяющий стакаться
    [Serializable]
    public sealed class StackableItemComponent : IItemComponent
    {
        public int Count = 1;
        public int MaxCount = 2;

        public IItemComponent Clone()
        {
            return new StackableItemComponent()
            {
                Count = Count,
                MaxCount = MaxCount
            };
        }
    }
}