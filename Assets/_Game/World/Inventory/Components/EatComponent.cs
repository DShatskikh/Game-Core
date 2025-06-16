namespace Game
{
    // Компонент предмета который помечает предмет как сьедобный
    public sealed class EatComponent : IItemComponent
    {
        public int Health;

        public IItemComponent Clone() =>
            new EatComponent() { Health = Health };
    }
}