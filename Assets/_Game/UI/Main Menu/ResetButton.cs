using Zenject;

namespace Game
{
    public sealed class ResetButton : BaseButton
    {
        private IAssetLoader _assetLoader;
        private IGameRepositoryStorage _gameRepositoryStorage;

        [Inject]
        private void Construct(IAssetLoader assetLoader, IGameRepositoryStorage gameRepositoryStorage)
        {
            _assetLoader = assetLoader;
            _gameRepositoryStorage = gameRepositoryStorage;
        }
        
        protected override void OnClick()
        {
            _gameRepositoryStorage.Reset();
            _assetLoader.LoadScene(AssetPathConstants.INTRO_SCENE_PATH);
        }
    }
}