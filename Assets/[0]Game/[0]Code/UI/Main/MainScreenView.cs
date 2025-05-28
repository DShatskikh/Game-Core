namespace Game
{
    // Визуальная часть основного окна игры
    public class MainScreenView : ScreenBase
    {
        public void ToggleActivate(bool isActive) => 
            gameObject.SetActive(isActive);
    }
}