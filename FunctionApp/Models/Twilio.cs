namespace FunctionApp.Models
{
    public class Twilio
    {
        public string Id { get; set; }

        public string Token { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public bool IsEnabled { get; set; }
    }
}