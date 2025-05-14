namespace Game
{
    // Логика окна
    public interface IScreenPresenter
    {
        public IScreenPresenter Prototype();
        public void Destroy();
    }
}