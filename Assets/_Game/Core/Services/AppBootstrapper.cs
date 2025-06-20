using FMODUnity;
using UnityEngine;

namespace Game
{
    public sealed class AppBootstrapper : MonoBehaviour
    {
        private IGameRepositoryStorage _gameRepositoryStorage;
        private IAssetLoader _assetLoader;

        private void Awake()
        {
            LoadGame();
        }

        private void LoadGame()
        {
#if UNITY_WEBGL
            if (!RuntimeManager.HasBankLoaded("Master"))
            {
                RuntimeManager.LoadBank("Master", true);
                RuntimeManager.LoadBank("Master.strings", true);
                RuntimeManager.WaitForAllSampleLoading();
            }
#endif

            _assetLoader = new AssetLoader();
            _gameRepositoryStorage = new MainRepositoryStorage();
            _gameRepositoryStorage.Load();
            
            _assetLoader.LoadScene(_gameRepositoryStorage.TryGet(SaveConstants.NOT_FIRST_GAME,
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