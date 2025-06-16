using UnityEngine;

namespace Game
{
    public sealed class EmptyAnalytics : IAnalyticsService
    {
        public void Send(string id) { }
        public void Send(string id, string message) { }
    }
}