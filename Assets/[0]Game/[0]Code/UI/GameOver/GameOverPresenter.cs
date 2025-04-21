using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class GameOverPresenter : IScreenPresenter, IGameGameOvertListener
    {
        private GameOverView _view;

        [Inject]
        private void Construct(GameOverView view)
        {
            _view = view;
        }

        public void OnGameOver()
        {
            _view.Show();
        }

        public IScreenPresenter Prototype() => 
            new GameOverPresenter();

        public void Destroy()
        {
        }

        public void SetMessage(string message)
        {
            _view.SetLabel(message);
        }
    }
}