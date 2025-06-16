using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // Точки команды игрока и команды противника
    public class BattlePoints : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _partyPoints;

        [SerializeField]
        private Transform[] _enemyPoints;
        
        public OverWorldPositionsData[] GetPartyPositionsData(Player player)
        {
            var squadOverWorldPositionsData = new List<OverWorldPositionsData>
            {
                new(player.transform, _partyPoints[0])
            };

            return squadOverWorldPositionsData.ToArray();
        }

        public OverWorldPositionsData[] GetEnemiesPositionsData(IEnemy[] enemies)
        {
            var enemiesOverWorldPositions = new List<OverWorldPositionsData>();

            for (var index = 0; index < enemies.Length; index++)
            {
                var enemy = enemies[index];
                enemiesOverWorldPositions.Add(new OverWorldPositionsData(((MonoBehaviour)enemy).transform,
                    _enemyPoints[index]));
            }

            return enemiesOverWorldPositions.ToArray();
        }
    }
}