namespace Game
{
    public sealed class ArmorService
    {
        private const int DEFAULT_ARMOR = 1;
        private readonly MainInventory _mainInventory;

        public int GetArmor
        {
            get
            {
                var attack = DEFAULT_ARMOR;

                if (_mainInventory.WeaponSlot.HasItem &&
                    _mainInventory.WeaponSlot.Item.TryGetComponent(out AttackComponent attackComponent))
                    attack += attackComponent.Attack;
                
                return attack;
            }
        }
        
        public ArmorService(MainInventory mainInventory)
        {
            _mainInventory = mainInventory;
        }
    }
}