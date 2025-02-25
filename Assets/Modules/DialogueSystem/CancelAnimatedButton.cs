using UnityEngine;

namespace Game
{
    public sealed class CancelAnimatedButton : MonoBehaviour
    {
        // private PlayerInput _playerInput;
        // private AnimatedButton _animatedButton;
        //
        // private void Awake()
        // {
        //     _playerInput = ServiceLocator.Get<PlayerInput>();
        //     _animatedButton = GetComponent<AnimatedButton>();
        // }
        //
        // private void OnEnable()
        // {
        //     _playerInput.actions["Cancel"].started += OnSubmitDown;
        //     _playerInput.actions["Cancel"].canceled += OnSubmitUp;
        // }
        //
        // private void OnDisable()
        // {
        //     if (_playerInput)
        //     {
        //         _playerInput.actions["Cancel"].started -= OnSubmitDown;
        //         _playerInput.actions["Cancel"].canceled -= OnSubmitUp;
        //     }
        // }
        //
        // private void OnSubmitDown(InputAction.CallbackContext obj)
        // {
        //     _animatedButton.OnPointerDown(null);
        // }
        //
        // private void OnSubmitUp(InputAction.CallbackContext obj)
        // {
        //     _animatedButton.OnPointerUp(null);
        //     _animatedButton.onClick?.Invoke();
        // }
    }
}