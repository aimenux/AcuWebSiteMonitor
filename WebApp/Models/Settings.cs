namespace WebApp.Models
{
    public class Settings
    {
        public int MaxHealthChecksRequests { get; set; } = 3;

        public int MaxHealthChecksEntries { get; set; } = 20;

        public int EvaluationTimeInSeconds { get; set; } = 10;

        public int NotificationTimeInSeconds { get; set; } = 30;

        public string Title { get; set; } = @"AcuWebSiteMonitor";

        public AcuWebSites AcuWebSites { get; set; } = new ();
    }
}
