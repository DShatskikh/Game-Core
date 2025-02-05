namespace Game
{
    public interface IGameTickableListener : IGameListener
    {
        void Tick(float delta);
    }
        
    public interface IGameFixedTickableListener : IGameListener
    {
        void FixedTick(float delta);
    }
}