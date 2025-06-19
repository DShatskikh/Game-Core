using System;
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
            public Slot[] MainSlots;
            public Slot ArmorSlot;
            public Slot HandSlot;
            public Slot WeaponSlot;
            public Slot WeaponAdditionalSlot;
        }

        [Inject]
        private IGameRepositoryStorage _gameRepositoryStorage;
        
        public const int SIZE = 16;
        
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

        public MainInventory()
        {
            for (int x = 0; x < SIZE; x++)
            {
                MainSlots[x] = new Slot(); 
            }
        }

        public void Add(Item item) => 
            InventoryUseCases.TryAddItem(this, item);

        public void PutOn(Slot slot) => 
            InventoryUseCases.PutOn(this, slot);

        public void EquipWeapon(Item weapon) => 
            InventoryUseCases.EquipWeapon(this, weapon);

        public void EquipAdditionalWeapon(Item additionalWeapon) => 
            InventoryUseCases.EquipAdditionalWeapon(this, additionalWeapon);

        public void EquipArmor(Item armor) => 
            InventoryUseCases.EquipArmor(this, armor);
        
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
            _gameRepositoryStorage.Set(SaveConstants.MAIN_INVENTORY, new Data()
            {
                ArmorSlot = ArmorSlot,
                HandSlot = HandSlot,
                MainSlots = MainSlots,
                WeaponAdditionalSlot = WeaponAdditionalSlot,
                WeaponSlot = WeaponSlot
            });
        }

        public void Load()
        {
            if (_gameRepositoryStorage.TryGet(SaveConstants.MAIN_INVENTORY, out Data data))
            {
                ArmorSlot = data.ArmorSlot;
                HandSlot = data.HandSlot;
                MainSlots = data.MainSlots;
                WeaponAdditionalSlot = data.WeaponAdditionalSlot;
                WeaponSlot = data.WeaponSlot;
            }
        }
    }
}