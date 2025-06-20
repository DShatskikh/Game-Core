namespace Game
{
    public sealed class AttackService
    {
        private const int DEFAULT_ATTACK = 1;
        private readonly MainInventory _mainInventory;
        public int GetAttack
        {
            get
            {
                var attack = DEFAULT_ATTACK;

                if (_mainInventory.WeaponSlot.HasItem &&
                    _mainInventory.WeaponSlot.Item.TryGetComponent(out AttackComponent attackComponent))
                    attack += attackComponent.Attack;

                if (attack > 1)
                    attack--;
                
                return attack;
            }
        }

        public AttackService(MainInventory mainInventory)
        {
            _mainInventory = mainInventory;
        }
    }
}