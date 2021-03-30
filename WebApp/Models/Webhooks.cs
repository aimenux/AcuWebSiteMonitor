using System;
using System.Collections.Generic;
using HealthChecks.UI.Core;

namespace WebApp.Models
{
    public class Webhooks : List<Webhook>
    {
    }

    public class Webhook : IWebhook
    {
        public string Name { get; set; } = null;

        public string Url { get; set; } = null;

        public string FailurePayload { get; set; } = "{\r\n \"@type\": \"MessageCard\",\r\n \"themeColor\": \"c60035\",\r\n \"title\": \"[[LIVENESS]] is failed.\",\r\n \"text\": \"[[FAILURE]]\",\r\n \"potentialAction\": [\r\n {\r\n \"@type\": \"OpenUri\",\r\n \"name\": \"Learn More\",\r\n \"targets\": [\r\n { \"os\": \"default\", \"uri\": \"https://localhost:44313/healthchecks-ui\" }\r\n ]\r\n }\r\n ]\r\n}";

        public string RestorePayload { get; set; } = "{\r\n \"@type\": \"MessageCard\",\r\n \"themeColor\": \"00c66a\",\r\n \"title\": \"[[LIVENESS]] is recovered.\",\r\n \"text\": \"There is 0 healthcheck failing.\",\r\n \"potentialAction\": [\r\n {\r\n \"@type\": \"OpenUri\",\r\n \"name\": \"Learn More\",\r\n \"targets\": [\r\n { \"os\": \"default\", \"uri\": \"https://localhost:44313/healthchecks-ui\" }\r\n ]\r\n }\r\n ]\r\n}";

        public Func<UIHealthReport, bool> ShouldNotifyFunc { get; set; } = null;
        
        public Func<UIHealthReport, string> CustomMessageFunc { get; set; } = null;
        
        public Func<UIHealthReport, string> CustomDescriptionFunc { get; set; } = null;
    }

    public interface IWebhook
    {
        string Name { get; }

        string Url { get; }

        string FailurePayload { get; }

        string RestorePayload { get; }

        Func<UIHealthReport, bool> ShouldNotifyFunc { get; }

        Func<UIHealthReport, string> CustomMessageFunc { get; }

        Func<UIHealthReport, string> CustomDescriptionFunc { get; }
    }
}
