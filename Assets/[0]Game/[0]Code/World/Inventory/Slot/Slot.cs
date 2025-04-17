using System;

namespace Game
{
    [Serializable]
    public class Slot
    {
        public Item Item;
        public bool HasItem => Item != null;
        public event Action<Item> OnSlotChange;
        public event Action<Item> OnItemAdded;
        public event Action<Item> OnItemRemoved;

        public void NotifyChange(Item item, Item previousItem)
        {
            if (previousItem != null)
                OnItemRemoved?.Invoke(previousItem);
            
            if (item != null)
                OnItemAdded?.Invoke(item);
            
            OnSlotChange?.Invoke(item);
        }

        public virtual bool CanMoveItem(Item item) => 
            true;

        public void RemoveItem() => 
            SlotUseCases.RemoveItem(this);
    }
}