namespace Game
{
    public class AddMaxHPComponent : IItemComponent
    {
        public int AddHealth;

        public IItemComponent Clone() =>
            new AddMaxHPComponent() { AddHealth = AddHealth };
    }
}