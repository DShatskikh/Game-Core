using System.Collections.Generic;
using YG;

namespace Game
{
    public sealed class YandexGamesAnalytics : IAnalyticsService
    {
        public void Send(string id)
        {
            YG2.MetricaSend(id);
        }

        public void Send(string id, string message)
        {
            YG2.MetricaSend(id, new Dictionary<string, object>
            {
                { id, message }
            });
        }
    }
}