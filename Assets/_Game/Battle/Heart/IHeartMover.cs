namespace Game
{
    // Базовый интерфейс движения сердца
    public interface IHeartMover
    {
        void Enable();
        void Disable();
        void Move();
        void FixedUpdate();
    }
}