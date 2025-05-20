using System;
using System.Collections.Generic;

namespace Game
{
    [Serializable]
    public struct DefeatedEnemiesSaveData
    {
        public HashSet<string> DefeatedEnemies;
        public HashSet<string> KilledEnemies;
    }
}