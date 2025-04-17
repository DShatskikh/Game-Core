namespace Game
{
    public interface IShell
    {
        bool IsDestroy { get; }
        bool IsAlive { get; set; }
        void Crash(Heart heart);
        void Hide();
    }
}