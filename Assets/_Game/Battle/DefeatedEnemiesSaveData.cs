using System;
using System.Collections.Generic;

namespace Game
{
    // Структура данных для сохранения побежденных и убитых противников
    [Serializable]
    public struct DefeatedEnemiesSaveData
    {
        public List<string> DefeatedEnemies;
        public List<string> KilledEnemies;
    }
}