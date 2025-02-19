using System.Collections.Generic;
using UniRx;

namespace Game
{
    public class CharacterInventory
    {
        public IReadOnlyReactiveCollection<ItemBaseConfig> Items => _items;
        public IReadOnlyReactiveCollection<WeaponItemConfig> Weapons => _weapons;
        public ReactiveProperty<ArmorItemConfig> Armor = new();
        public int MaxCount => _maxCount;

        private ReactiveCollection<ItemBaseConfig> _items = new();
        private ReactiveCollection<WeaponItemConfig> _weapons = new();
        private readonly int _maxCount = 8;

        public void AddItem(ItemBaseConfig item) => 
            _items.Add(item);

        public void RemoveItem(ItemBaseConfig item) => 
            _items.Remove(item);

        public void SetItems(IEnumerable<ItemBaseConfig> items) => 
            _items = new ReactiveCollection<ItemBaseConfig>(items);

        public void AddWeapon(WeaponItemConfig weapon) => 
            _weapons.Add(weapon);

        public void RemoveWeapon(WeaponItemConfig weapon) => 
            _weapons.Remove(weapon);

        public void SetWeapons(IEnumerable<WeaponItemConfig> weapons) => 
            _weapons = new ReactiveCollection<WeaponItemConfig>(weapons);
    }
}