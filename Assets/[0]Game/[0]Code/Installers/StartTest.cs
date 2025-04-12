using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StartTest : MonoBehaviour
    {
        private ScreenManager _screenManager;
        private DiContainer _diContainer;

        [Inject]
        public void Container(ScreenManager screenManager, DiContainer diContainer)
        {
            _screenManager = screenManager;
            _diContainer = diContainer;
        }

        private void Start()
        {
            _screenManager.Open(ScreensEnum.MAIN, _diContainer);
        }
    }
}