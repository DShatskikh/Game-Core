using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public sealed class MainMenu : ScreenBase
    {
        [SerializeField]
        private ShopButton _startGameButton;

        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(_startGameButton.gameObject);
        }
    }
}