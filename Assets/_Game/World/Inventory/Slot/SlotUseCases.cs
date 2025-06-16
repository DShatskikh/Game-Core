using System.Collections.Generic;

namespace Game
{
    // Расспространенные сценарии использования слота
    public static class SlotUseCases
    {
        public static bool TryRemoveOneItem(Slot slot)
        {
            var previousItem = slot.Item;
            
            if (!ItemUseCases.CanFlag(slot.Item, ItemFlags.STACKABLE))
            {
                slot.Item = null;
                slot.NotifyChange(slot.Item, previousItem);
                return true;
            }
            else if (ItemUseCases.TryGetComponent(slot.Item, out StackableItemComponent stackableItemComponent))
            {
                stackableItemComponent.Count -= 1;

                if (stackableItemComponent.Count <= 0)
                {
                    slot.Item = null;
                }
                
                slot.NotifyChange(slot.Item, previousItem);
                return true;
            }
            
            return false;
        }

        public static bool TrySwitch(Slot slot, Slot addedSlot)
        {
            if (!slot.CanMoveItem(addedSlot.Item))
                return false;

            if (slot.HasItem && addedSlot.HasItem && slot.Item.ID == addedSlot.Item.ID
                && ItemUseCases.CanFlag(slot.Item, ItemFlags.STACKABLE) 
                && slot.Item.TryGetComponent(out StackableItemComponent stackableItemComponent) 
                && addedSlot.Item.TryGetComponent(out StackableItemComponent addedStackableComponent)
                && stackableItemComponent.Count != stackableItemComponent.MaxCount)
            {
                var previousItem = addedSlot.Item;
                
                stackableItemComponent.Count += addedStackableComponent.Count;
                
                if (stackableItemComponent.Count > stackableItemComponent.MaxCount)
                {
                    addedStackableComponent.Count = stackableItemComponent.Count % stackableItemComponent.MaxCount;
                    stackableItemComponent.Count = stackableItemComponent.MaxCount;
                }
                else
                {
                    addedStackableComponent.Count = 0;
                    addedSlot.Item = null;
                }

                slot.NotifyChange(slot.Item, slot.Item);
                addedSlot.NotifyChange(addedSlot.Item, previousItem);
                return true;
            }

            (slot.Item, addedSlot.Item) = (addedSlot.Item, slot.Item);
            slot.NotifyChange(slot.Item, addedSlot.Item);
            addedSlot.NotifyChange(addedSlot.Item, slot.Item);
            return true;
        }
        
        public static bool TryAdd(Slot slot, Item addedItem, out int remains)
        {
            remains = 0;

            if (slot.HasItem)
            {
                if (slot.Item.ID == addedItem.ID)
                {
                    if (ItemUseCases.CanFlag(slot.Item, ItemFlags.STACKABLE))
                    {
                        if (ItemUseCases.TryGetComponent(slot.Item, out StackableItemComponent stackableItemComponent) 
                            && ItemUseCases.TryGetComponent(addedItem, out StackableItemComponent addedStackableItemComponent))
                        {
                            stackableItemComponent.Count += addedStackableItemComponent.Count;

                            if (stackableItemComponent.Count > stackableItemComponent.MaxCount)
                            {
                                addedStackableItemComponent.Count = stackableItemComponent.Count % stackableItemComponent.MaxCount;
                                stackableItemComponent.Count = stackableItemComponent.MaxCount;
                            }
                            else
                            {
                                addedStackableItemComponent.Count = 0;
                            }
                            
                            remains = addedStackableItemComponent.Count;
                            slot.NotifyChange(slot.Item, slot.Item);
                            return true;
                        }
                    }
                }

                return false;
            }
            else
            {
                var previousItem = slot.Item;
                slot.Item = addedItem;
                slot.NotifyChange(slot.Item, previousItem);
                return true; 
            }
        }

        public static void TakeHalf(Slot slot, Slot moveSlot)
        {
            if (!ItemUseCases.CanFlag(slot.Item, ItemFlags.STACKABLE) 
                || (ItemUseCases.CanFlag(slot.Item, ItemFlags.STACKABLE) 
                    && ItemUseCases.TryGetComponent(slot.Item, out StackableItemComponent stackableItemComponent) 
                    && stackableItemComponent.Count == 1))
            {
                moveSlot.Item = slot.Item;
                slot.Item = null;
                
                moveSlot.NotifyChange(moveSlot.Item, null);
                slot.NotifyChange(null,  moveSlot.Item);
                return;
            }

            if (ItemUseCases.TryGetComponent(slot.Item, out StackableItemComponent stackableItemComponent1))
            {
                moveSlot.Item = slot.Item.Clone();
                moveSlot.Item.TryGetComponent(out StackableItemComponent moveStackableItemComponent);
                
                stackableItemComponent1.Count /= 2;
                moveStackableItemComponent.Count = moveStackableItemComponent.Count / 2 + moveStackableItemComponent.Count % 2;
                
                moveSlot.NotifyChange(moveSlot.Item, null);
                slot.NotifyChange(slot.Item,  slot.Item);
            }
        }

        public static bool TryPutOneItemDown(Slot slot, Slot moveSlot)
        {
            if (!slot.HasItem)
            {
                if (!ItemUseCases.CanFlag(moveSlot.Item, ItemFlags.STACKABLE))
                {
                    (slot.Item, moveSlot.Item) = (moveSlot.Item, slot.Item);
                
                    slot.NotifyChange(slot.Item, moveSlot.Item);
                    moveSlot.NotifyChange(moveSlot.Item, slot.Item);
                    return true;
                }
                else if (ItemUseCases.CanFlag(moveSlot.Item, ItemFlags.STACKABLE) 
                         && ItemUseCases.TryGetComponent(moveSlot.Item, out StackableItemComponent moveStackableItemComponent))
                {
                    var previousItem = moveSlot.Item;
                
                    moveStackableItemComponent.Count -= 1;
                    slot.Item = moveSlot.Item.Clone();
                    ItemUseCases.TryGetComponent(slot.Item, out StackableItemComponent stackableItemComponent);
                    stackableItemComponent.Count = 1;

                    if (moveStackableItemComponent.Count <= 0)
                    {
                        moveSlot.Item = null;
                    }
                
                    slot.NotifyChange(slot.Item, slot.Item);
                    moveSlot.NotifyChange(moveSlot.Item, previousItem);
                    return true;
                }
            }
            else if (slot.Item.ID == moveSlot.Item.ID 
                     && ItemUseCases.CanFlag(moveSlot.Item, ItemFlags.STACKABLE) 
                     && ItemUseCases.TryGetComponent(moveSlot.Item, out StackableItemComponent moveStackableItemComponent)
                     && ItemUseCases.TryGetComponent(slot.Item, out StackableItemComponent stackableItemComponent)
                     && stackableItemComponent.Count < stackableItemComponent.MaxCount)
            {
                var previousItem = moveSlot.Item;
                
                moveStackableItemComponent.Count -= 1;
                stackableItemComponent.Count += 1;

                if (moveStackableItemComponent.Count <= 0)
                {
                    moveSlot.Item = null;
                }
                
                slot.NotifyChange(slot.Item, slot.Item);
                moveSlot.NotifyChange(moveSlot.Item, previousItem);
                return true;
            }

            return false;
        }

        public static void PutAllItem(Slot moveSlot, List<Slot> allSlots)
        {
            foreach (var slot in allSlots)
            {
                if (!slot.HasItem)
                    continue;

                if (moveSlot.Item.ID == slot.Item.ID && ItemUseCases.TryGetComponent(moveSlot.Item, out StackableItemComponent stackableItemComponent))
                {
                    if (stackableItemComponent.Count == stackableItemComponent.MaxCount)
                        return;
                    
                    TrySwitch(moveSlot, slot);

                    if (stackableItemComponent.Count == stackableItemComponent.MaxCount)
                        return;
                }
            }
        }

        public static void RemoveItem(Slot slot)
        {
            slot.Item = null;
        }
    }
}