using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace StepSound
{
    // Конфиг хранящий тип тайла
    [CreateAssetMenu(fileName = "TileTagConfig", menuName = "Data/TileTagConfig", order = 81)]
    public class TileTagConfig : ScriptableObject
    {
        public List<TileBase> Stone;
        public List<TileBase> Grass;
        public List<TileBase> Dirt;
        public List<TileBase> Wood;
        public List<TileBase> Sponge;
        public List<TileBase> Sand;

        public float GetPair(TileBase tile)
        {
            if (Stone.Any(currentTile => tile == currentTile))
                return 0;

            if (Grass.Any(currentTile => tile == currentTile))
                return 1;

            if (Dirt.Any(currentTile => tile == currentTile))
                return 2;

            if (Wood.Any(currentTile => tile == currentTile))
                return 3;
            
            if (Sponge.Any(currentTile => tile == currentTile))
                return 4;
            
            if (Sand.Any(currentTile => tile == currentTile))
                return 5;

            return 6;
        }
    }
}