using UnityEngine;

namespace Game
{
    public class TestEnemyController1 : MonoBehaviour, IEnemyController
    {
        [SerializeField]
        private TestEnemy _testEnemy;

        public IEnemy[] GetEnemies => new IEnemy[] { _testEnemy };
    }
}