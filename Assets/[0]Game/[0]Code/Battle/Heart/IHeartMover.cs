namespace Game
{
    public interface IHeartMover
    {
        void Enable();
        void Disable();
        void Move();
        void FixedUpdate();
    }
}