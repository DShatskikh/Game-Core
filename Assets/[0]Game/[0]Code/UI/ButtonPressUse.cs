using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    public class ButtonPressUse : MonoBehaviour
    {
        private Player _player;
        private PlayerInput _playerInput;
        private Button _button;

        [Inject]
        private void Construct(Player player, PlayerInput playerInput)
        {
            _player = player;
            _playerInput = playerInput;
            
            _button = GetComponent<Button>();
            
            _button.onClick.AddListener(OnClick);

            _player.NearestUseObject.Subscribe(_ =>
            {
                gameObject.SetActive(_ != null);
            });
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnClick()
        {
            gameObject.SetActive(false);
            _player.TryUse(new InputAction.CallbackContext());
        }
    }
}