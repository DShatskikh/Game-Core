using UniRx;

namespace Game
{
    public class CharacterInventory
    {
        public IReadOnlyReactiveCollection<ItemBaseConfig> Items => _items;
        public ReactiveProperty<WeaponItemConfig> Weapon = new();
        public ReactiveProperty<ArmorItemConfig> Armor = new();
        public int MaxCount => _maxCount;

        private ReactiveCollection<ItemBaseConfig> _items = new();
        private readonly int _maxCount = 8;

        public void AddItem(ItemBaseConfig item)
        {
            _items.Add(item);
        }

        public void RemoveItem(ItemBaseConfig item)
        {
            _items.Remove(item);
        }

        public void SetItems(ItemBaseConfig[] items)
        {
            _items = new ReactiveCollection<ItemBaseConfig>(items);
        }
    }
}