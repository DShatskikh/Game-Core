namespace Game
{
    // Расспространенные сценарии использования предмета
    public static class ItemUseCases
    {
        public static bool CanFlag(Item item, ItemFlags flag) => 
            (item.Flags & flag) == flag;
        
        public static bool TryGetComponent<T>(Item item, out T component) where T : IItemComponent
        {
            component = default;

            if (item == null)
                return false;

            if (item.Components == null)
                return false;
            
            foreach (var itemComponent in item.Components)
            {
                if (itemComponent.GetType() == typeof(T))
                {
                    component = (T)itemComponent;
                    return true;
                }
            }

            return false;
        }

        public static bool CanComponent(Item item, IItemComponent component)
        {
            if (item == null)
                return false;

            foreach (var currentComponent in item.Components)
            {
                if (currentComponent == component)
                    return true;
            }

            return false;
        }
    }
}