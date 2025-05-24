using System;
using System.Collections.Generic;

namespace Game
{
    [Serializable]
    public struct DefeatedEnemiesSaveData
    {
        public List<string> DefeatedEnemies;
        public List<string> KilledEnemies;
    }
}