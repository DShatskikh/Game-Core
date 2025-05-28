using QFSW.QC;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    // Обработчик открытия консоли
    public sealed class ConsoleToggleHandler
    {
        private readonly PlayerInput _input;
        private readonly QuantumConsole _prefab;
        
        private QuantumConsole _console;

        public ConsoleToggleHandler(PlayerInput input, QuantumConsole prefab)
        {
            _input = input;
            _prefab = prefab;
            
            input.actions["OpenConsole"].started += Toggle;
        }

        ~ConsoleToggleHandler()
        {
            if (_input)
                _input.actions["OpenConsole"].started -= Toggle;
        }
        
        private void Toggle(InputAction.CallbackContext context)
        {
            if (_console == null)
            {
                _console = Object.Instantiate(_prefab);
            }
            else
            {
                Object.Destroy(_console.gameObject);
            }
        }
    }
}