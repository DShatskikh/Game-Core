namespace Game
{
    public sealed class GameOverPresenter : IGameGameOvertListener
    {
        private readonly GameOverView _view;

        public GameOverPresenter(GameOverView view)
        {
            _view = view;
        }

        public void OnGameOver()
        {
            _view.Show();
        }
    }
}