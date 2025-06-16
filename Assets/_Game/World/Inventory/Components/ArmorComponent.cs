using System;

namespace Game
{
    // Компонент предмета который хранит броню
    [Serializable]
    public sealed class ArmorComponent : IItemComponent
    {
        public int Armor;

        public IItemComponent Clone() => 
            new ArmorComponent() { Armor = Armor };
    }
}