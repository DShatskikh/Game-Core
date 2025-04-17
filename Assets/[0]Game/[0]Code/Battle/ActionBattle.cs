using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public struct ActionBattle
    {
        public string Name; //LocalizedString
        public string Reaction;
        
        [Range(0, 100)]
        public int Progress;
    }
}