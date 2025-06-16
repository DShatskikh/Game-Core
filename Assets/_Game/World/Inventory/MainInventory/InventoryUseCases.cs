using UnityEngine;

namespace Game
{
    // Расспространенные сценарии использования инвентаря
    public static class InventoryUseCases
    {
        public static bool TryAddItem(MainInventory inventory, Item item)
        {
            var mainSlots = inventory.MainSlots;
            
            for (int x = 0; x < mainSlots.Length; x++)
            {
                var slot = mainSlots[x];
                    
                if (SlotUseCases.TryAdd(slot, item, out int remains))
                {
                    if (remains == 0)
                        return true;
                }
            } 

            Debug.Log("Предмет не поместился");
            return false;
        }

        public static void PutOn(MainInventory mainInventory, Slot slot)
        {
            (slot.Item, mainInventory.ArmorSlot.Item) = (mainInventory.ArmorSlot.Item, slot.Item);
            mainInventory.NotifyChangeSlot(slot.Item, mainInventory.ArmorSlot.Item,slot);
            mainInventory.NotifyChangeSlot(slot.Item, mainInventory.ArmorSlot.Item, mainInventory.ArmorSlot);
        }

        public static void EquipWeapon(MainInventory mainInventory, Item weapon)
        {
            mainInventory.WeaponSlot.Item = weapon.Clone();
        }

        public static void EquipArmor(MainInventory mainInventory, Item armor)
        {
            mainInventory.ArmorSlot.Item = armor.Clone();
        }

        public static void EquipAdditionalWeapon(MainInventory mainInventory, Item additionalWeapon)
        {
            mainInventory.WeaponAdditionalSlot.Item = additionalWeapon.Clone();
        }

        public static bool IsGetItem(MainInventory mainInventory, string id)
        {
            foreach (var slot in mainInventory.MainSlots)
            {
                if (slot.HasItem && slot.Item.ID == id)
                    return true;
            }

            return false;
        }

        public static bool TryRemoveItem(MainInventory mainInventory, string id)
        {
            foreach (var slot in mainInventory.MainSlots)
            {
                if (slot.HasItem && slot.Item.ID == id)
                {
                    return SlotUseCases.TryRemoveOneItem(slot);
                }
            }

            return false;
        }
    }
}