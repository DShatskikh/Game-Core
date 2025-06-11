using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class SwordView : MonoBehaviour
    {
        private MainInventory _inventory;
        private SpriteRenderer _spriteRenderer;

        [Inject]
        private void Construct(MainInventory inventory)
        {
            _inventory = inventory;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            if (_inventory.WeaponSlot.HasItem && _inventory.WeaponSlot.Item.TryGetComponent(out AttackComponent attackComponent))
                _spriteRenderer.sprite = attackComponent.WeaponSprite;
        }
    }
}