using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class AppBootstrapper : MonoBehaviour
    {
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
                // Загружаем сцену с меню
                _assetLoader.LoadScene(AssetPathConstants.MENU_SCENE_PATH);
            }
            else // 1 вход в игру
            {
                // Загружаем катсцену
                _assetLoader.LoadScene(AssetPathConstants.INTRO_SCENE_PATH);
            }
        }
    }
}