namespace Game
{
    public interface IAnalyticsService
    {
        void Send(string id);
        void Send(string id, string message);
    }
}