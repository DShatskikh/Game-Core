using Zenject;

namespace Game
{
    public sealed class ContinueButton : BaseButton
    {
        private IAssetLoader _assetLoader;

        [Inject]
        private void Construct(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }
        
        protected override void OnClick()
        {
            _assetLoader.LoadScene(AssetPathConstants.GAME_SCENE_PATH);
        }
    }
}