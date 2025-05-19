using System;

namespace Game
{
    [Serializable]
    public sealed class MainInventory
    {
        public const int SIZE = 16;
        
        public Slot[] MainSlots = new Slot[SIZE];
        public Slot ArmorSlot = new();
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
    }
}