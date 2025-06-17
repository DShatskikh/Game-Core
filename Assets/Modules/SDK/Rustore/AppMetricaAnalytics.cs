using Io.AppMetrica;

namespace Game
{
    public class AppMetricaAnalytics : IAnalyticsService
    {
        public AppMetricaAnalytics()
        {
            AppMetrica.Activate(new AppMetricaConfig("TEST") {
                Logs = true,
            });
        }
        
        public void Send(string id)
        {
            AppMetrica.ReportEvent(id);
        }

        public void Send(string id, string message)
        {
            AppMetrica.ReportEvent(id, message);
        }
    }
}