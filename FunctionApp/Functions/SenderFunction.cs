using System.IO;
using System.Net;
using System.Threading.Tasks;
using FunctionApp.Models;
using FunctionApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FunctionApp.Functions
{
    public class SenderFunction
    {
        private const string QueryParameter = "payload";

        private readonly ISender _sender;

        public SenderFunction(ISender sender)
        {
            _sender = sender;
        }

        [FunctionName(nameof(SenderFunction))]
        [OpenApiOperation(operationId: "Run", tags: new[] { QueryParameter })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: QueryParameter, In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Payload** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var notification = await ExtractNotificationAsync(req);
            log.LogInformation("[{@notification}]", notification);
            await _sender.SendAsync(notification);
            return new OkObjectResult(notification);
        }

        private static async Task<Notification> ExtractNotificationAsync(HttpRequest req)
        {
            if (!TryGetPayloadFromQueryString(req, out var payload))
            {
                using var stream = new StreamReader(req.Body);
                payload = await stream.ReadToEndAsync();
            }

            return JsonConvert.DeserializeObject<Notification>(payload);
        }

        private static bool TryGetPayloadFromQueryString(HttpRequest req, out string payload)
        {
            payload = req.Query[QueryParameter];
            return !string.IsNullOrWhiteSpace(payload);
        }
    }
}

