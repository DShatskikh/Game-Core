namespace Game
{
    public sealed class EatComponent : IItemComponent
    {
        public int Health;

        public IItemComponent Clone() =>
            new EatComponent() { Health = Health };
    }
}