using Zenject;

namespace Game
{
    // Кнопка начала игры
    public sealed class StartGameButton : BaseButton
    {
        private IAssetLoader _assetLoader;

        [Inject]
        private void Construct(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }
        
        protected override void OnClick()
        {
            _assetLoader.LoadScene(AssetPathConstants.INTRO_SCENE_PATH);
        }
    }
}