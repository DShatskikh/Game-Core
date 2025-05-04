using UnityEngine.SceneManagement;

namespace Game
{
    public sealed class StartGameButton : BaseButton
    {
        protected override void OnClick()
        {
            SceneManager.LoadScene(2);
        }
    }
}