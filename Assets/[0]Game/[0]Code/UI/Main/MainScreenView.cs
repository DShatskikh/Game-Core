namespace Game
{
    public class MainScreenView : ScreenBase
    {
        public void ToggleActivate(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}