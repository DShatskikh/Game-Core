using UnityEngine;

namespace Game
{
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

        public static void EquipWeapon(MainInventory mainInventory, ItemConfig weapon)
        {
            mainInventory.WeaponSlot.Item = weapon.Prototype.Clone();
        }

        public static void EquipAdditionalWeapon(MainInventory mainInventory, ItemConfig additionalWeapon)
        {
            mainInventory.WeaponAdditionalSlot.Item = additionalWeapon.Prototype.Clone();
        }
    }
}