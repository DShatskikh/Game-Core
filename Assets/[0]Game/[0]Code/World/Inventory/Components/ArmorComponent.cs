using System;

namespace Game
{
    [Serializable]
    public sealed class ArmorComponent : IItemComponent
    {
        public int Armor;

        public IItemComponent Clone() => 
            new ArmorComponent() { Armor = Armor };
    }
}