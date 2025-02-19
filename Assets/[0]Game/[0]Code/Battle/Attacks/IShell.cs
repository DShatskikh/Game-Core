namespace Game
{
    public interface IShell
    {
        bool IsDestroy { get; }
        void Crash(Heart heart);
        void Hide();
    }
}