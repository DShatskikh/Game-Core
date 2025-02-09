using System;

namespace Game
{
    public class TestEnemyController : IEnemyController
    {
        public IEnemy[] GetEnemies => new IEnemy[0];
    }
}