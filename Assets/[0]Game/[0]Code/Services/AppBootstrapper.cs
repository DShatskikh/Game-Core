using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class AppBootstrapper : MonoBehaviour
    {
        private const string SCENE_PATH = "Assets/[0]Game/[3]Scenes/";
        
        private IGameRepositoryStorage _gameRepositoryStorage;
        private IAssetLoader _assetLoader;

        [Inject]
        private void Construct(IGameRepositoryStorage gameRepositoryStorage,
            IAssetLoader assetLoader)
        {
            _gameRepositoryStorage = gameRepositoryStorage;
            _assetLoader = assetLoader;

            LoadGame();
        }

        private void LoadGame()
        {
            // не 1 вход в игру
            if (_gameRepositoryStorage.TryGet(SaveConstants.NotFirstGame,
                    out MarkerData _))
            {
                // Загружаем сцену с игрой
                _assetLoader.LoadScene(SCENE_PATH + "Game.unity");
            }
            else // 1 вход в игру
            {
                // Загружаем катсцену
                _assetLoader.LoadScene(SCENE_PATH + "Intro.unity");
            }
        }
    }
}