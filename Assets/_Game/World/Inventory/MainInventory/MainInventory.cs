using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game
{
    // Инвентарь игрока
    [Serializable]
    public sealed class MainInventory
    {
        [Serializable]
        public struct Data
        {
            public Item[] MainItems;
            public Item ArmorItem;
            public Item HandItem;
            public Item WeaponItem;
            public Item WeaponAdditionalItem;
        }

        public const int SIZE = 16;
        
        private IGameRepositoryStorage _gameRepositoryStorage;

        public Slot[] MainSlots = new Slot[SIZE];
        public int GetItemsCount
        {
            get
            {
                var count = 0;
                
                foreach (var slot in MainSlots)
                {
                    if (slot.HasItem)
                        count++;
                }

                return count;
            }
        }

        public Slot ArmorSlot = new();
        public Slot HandSlot = new();
        public Slot WeaponSlot = new();
        public Slot WeaponAdditionalSlot = new();
        
        public event Action<Item> OnItemAdded; 
        public event Action<Item> OnItemRemoved; 
        public event Action<Item> OnItemConsumed;
        public event Action<Item, Slot> OnSlotChange;

        public MainInventory(IGameRepositoryStorage gameRepositoryStorage)
        {
            _gameRepositoryStorage = gameRepositoryStorage;
        }

        public void Add(Item item)
        {
            InventoryUseCases.TryAddItem(this, item);
            Save();
        }

        public void PutOn(Slot slot)
        {
            InventoryUseCases.PutOn(this, slot);
            Save();
        }

        public void EquipWeapon(Item weapon)
        {
            InventoryUseCases.EquipWeapon(this, weapon);
            Save();
        }

        public void EquipAdditionalWeapon(Item additionalWeapon)
        {
            InventoryUseCases.EquipAdditionalWeapon(this, additionalWeapon);
            Save();
        }

        public void EquipArmor(Item armor)
        {
            InventoryUseCases.EquipArmor(this, armor);
            Save();
        }

        public void NotifyAddItem(Item item) => 
            OnItemAdded?.Invoke(item);

        public void NotifyRemoveItem(Item item) => 
            OnItemRemoved?.Invoke(item);

        public void NotifyConsumeItem(Item item) => 
            OnItemConsumed?.Invoke(item);

        public void NotifyChangeSlot(Item item, Item previousItem, Slot slot)
        {
            if (previousItem != null)
                OnItemRemoved?.Invoke(previousItem);
            
            OnItemAdded?.Invoke(item);
            OnSlotChange?.Invoke(item, slot);
        }

        public bool IsGetItem(string id) => 
            InventoryUseCases.IsGetItem(this, id);

        public bool TryRemoveItem(string id) => 
            InventoryUseCases.TryRemoveItem(this, id);

        public void Save()
        {
            for (int i = 0; i < MainSlots.Length; i++) 
                MainSlots[i] ??= new Slot();

            _gameRepositoryStorage.Set(SaveConstants.MAIN_INVENTORY, new Data()
            {
                ArmorItem = ArmorSlot.Item,
                HandItem = HandSlot.Item,
                MainItems = MainSlots.Select(x => x.Item).ToArray(),
                WeaponAdditionalItem = WeaponAdditionalSlot.Item,
                WeaponItem = WeaponSlot.Item
            });
        }

        public void Load()
        {
            Debug.Log("Загрузили предметы");
            
            for (int x = 0; x < SIZE; x++)
            {
                MainSlots[x] = new Slot();
            }
            
            if (_gameRepositoryStorage.TryGet(SaveConstants.MAIN_INVENTORY, out Data data))
            {
                ArmorSlot.Item = (data.ArmorItem == null || data.ArmorItem.ID == string.Empty) ? null : data.ArmorItem;
                HandSlot.Item = (data.HandItem == null || data.HandItem.ID == string.Empty) ? null : data.HandItem;
                    
                MainSlots = new Slot[data.MainItems.Length];

                for (int i = 0; i < data.MainItems.Length; i++)
                {
                    if (MainSlots[i] == null)
                        MainSlots[i] = new Slot();
                    
                    MainSlots[i].Item = (data.MainItems[i] == null || data.MainItems[i].ID == string.Empty) ? null : data.MainItems[i];
                }

                WeaponAdditionalSlot.Item = (data.WeaponAdditionalItem == null || data.WeaponAdditionalItem.ID == string.Empty) ? null : data.WeaponAdditionalItem;
                WeaponSlot.Item = (data.WeaponItem == null || data.WeaponItem.ID == string.Empty) ? null : data.WeaponItem;
            }
        }
    }
}