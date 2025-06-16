using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class EnderChest : MonoBehaviour, IUseObject, IGameEnderChestListener
    {
        [SerializeField]
        private Sprite _open, _close;
        
        private SpriteRenderer _spriteRenderer;
        private GameStateController _gameStateController;
        private bool _isOpened;

        [Inject]
        private void Construct(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
            
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Use()
        {
            _spriteRenderer.sprite = _open;
            _isOpened = true;
            _gameStateController.OpenEnderChest();
            Debug.Log("Open Ender Chest");
        }

        public void OnOpenEnderChest() { }

        public void OnCloseEnderChest()
        {
            if (!_isOpened)
                return;
            
            _isOpened = false;
            _spriteRenderer.sprite = _close;
        }
    }
}