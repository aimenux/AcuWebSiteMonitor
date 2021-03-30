using System;
using System.Linq;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApp.Models
{
    public class AcuWebSite
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string ConnectionString { get; set; }

        private string _endpointUrl;

        public string EndpointUrl
        {
            get { return _endpointUrl ??= $"/healthchecks/{Name}"; }
        }

        private HealthCheckOptions _options;

        public HealthCheckOptions Options
        {
            get
            {
                return _options ??= new HealthCheckOptions
                {
                    Predicate = setup => setup.Tags.Any(x => x.Equals(Name, StringComparison.OrdinalIgnoreCase)),
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                };
            }
        }
    }
}