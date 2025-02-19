using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BattlePoints : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _partyPoints;

        [SerializeField]
        private Transform[] _enemyPoints;
        
        public Transform[] GetPartyPoints => _partyPoints;
        public Transform[] GetEnemyPoints => _enemyPoints;
        
        public OverWorldPositionsData[] GetPartyPositionsData(Player player)
        {
            var squadOverWorldPositionsData = new List<OverWorldPositionsData>
            {
                new(player.transform, _partyPoints[0])
            };

            /*foreach (var companion in GameData.CompanionsManager.GetAllCompanions)
            {
                squadOverWorldPositionsData.Add(new OverWorldPositionsData(companion.transform, 
                    SquadPoints[index], companion.GetSpriteRenderer.sprite));
                
                index++;
            }*/

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