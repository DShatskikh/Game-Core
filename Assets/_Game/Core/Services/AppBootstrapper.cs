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
            _assetLoader.LoadScene(_gameRepositoryStorage.TryGet(SaveConstants.NotFirstGame,
                out MarkerData _)
            
                // не 1 вход в игру
                // Загружаем сцену с меню
                ? AssetPathConstants.MENU_SCENE_PATH
                
                // 1 вход в игру
                // Загружаем катсцену
                : AssetPathConstants.INTRO_SCENE_PATH);
        }
    }
}