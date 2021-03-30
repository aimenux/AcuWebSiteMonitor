namespace WebApp.Models
{
    public class Settings
    {
        public Webhooks Webhooks { get; set; } = new();

        public AcuWebSites AcuWebSites { get; set; } = new ();
    }
}
