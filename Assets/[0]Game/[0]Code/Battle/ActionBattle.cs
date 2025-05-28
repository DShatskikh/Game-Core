using System;
using I2.Loc;
using UnityEngine;

namespace Game
{
    // Структура которая хранит действие во время боя
    [Serializable]
    public struct ActionBattle
    {
        public string Name;
        public string Info;
        public string Reaction;

        [Range(0, 100)]
        public int Progress;
    }
}