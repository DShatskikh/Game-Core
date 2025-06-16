namespace Game
{
    // Компонент предмета который увеличивает максимальное колличество здоровья
    public class AddMaxHPComponent : IItemComponent
    {
        public int AddHealth;

        public IItemComponent Clone() =>
            new AddMaxHPComponent() { AddHealth = AddHealth };
    }
}