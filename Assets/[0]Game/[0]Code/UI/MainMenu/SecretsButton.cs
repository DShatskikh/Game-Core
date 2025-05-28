using UnityEngine;

namespace Game
{
    // Кнопка которая открывает окно с секретами
    public sealed class SecretsButton : BaseButton
    {
        [SerializeField]
        private SecretsScreen _secretsScreen;

        [SerializeField]
        private MainMenu _mainMenu;
        
        protected override void OnClick()
        {
            _mainMenu.gameObject.SetActive(false);
            _secretsScreen.gameObject.SetActive(true);
        }
    }
}