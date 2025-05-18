using System;
using UnityEngine.InputSystem;
using Zenject;

namespace Game
{
    public sealed class MenuButton : BaseButton
    {
        private ScreenManager _screenManager;
        private DiContainer _diContainer;
        private PlayerInput _input;

        [Inject]
        private void Construct(ScreenManager screenManager, DiContainer diContainer, PlayerInput input)
        {
            _screenManager = screenManager;
            _diContainer = diContainer;
            _input = input;
            
            _input.actions["Menu"].performed += Onperformed;
        }

        private void OnDestroy()
        {
            _input.actions["Menu"].performed -= Onperformed;
        }

        protected override void OnClick()
        {
            _input.actions["Menu"].performed -= Onperformed;
            
            _screenManager.Close(ScreensEnum.MAIN);
            _screenManager.Open(ScreensEnum.MENU, _diContainer);
        }

        private void Onperformed(InputAction.CallbackContext obj)
        {
            OnClick();
        }
    }
}