using System.Collections;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Starter : MonoBehaviour
    {
        [SerializeField]
        private TestEnemyController1 _enemyController1;
        
        private GameStateController _gameStateController;
        private DiContainer _diContainer;

        [Inject]
        private void Construct(GameStateController gameStateController, DiContainer diContainer)
        {
            _gameStateController = gameStateController;
            _diContainer = diContainer;
        }
        
        public IEnumerator Start()
        {
            yield return null;
            _gameStateController.StartGame();

            var battleController = Instantiate(AssetProvider.Instance.BattleController);
            //_diContainer.Inject(battleController.gameObject);
            battleController.GetComponent<GameObjectContext>().Install(_diContainer);
            battleController.SetEnemyController(_enemyController1);
        }
    }
}