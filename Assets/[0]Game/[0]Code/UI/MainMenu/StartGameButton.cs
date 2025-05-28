using UnityEngine.SceneManagement;

namespace Game
{
    // Кнопка начала игры
    public sealed class StartGameButton : BaseButton
    {
        protected override void OnClick()
        {
            SceneManager.LoadScene(2);
        }
    }
}