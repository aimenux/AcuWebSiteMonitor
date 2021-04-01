using Newtonsoft.Json;

namespace FunctionApp.Models
{
    public class Notification
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{Title} {Text}".Trim();
        }
    }
}
