using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApp.Checkers;
using WebApp.Models;
using WebApp.Publishers;
using HealthChecksUiSettings = HealthChecks.UI.Configuration.Settings;

namespace WebApp
{
    public static class Extensions
    {
        public static IHealthChecksBuilder AddUrls(
            this IHealthChecksBuilder builder,
            AcuWebSites acuWebSites,
            TimeSpan? timeout = null)
        {
            return acuWebSites.Aggregate(builder,
                (current, acuWebSite) => current.AddUrlGroup(name: $"Url [{acuWebSite.Name}]",
                    uri: new Uri(acuWebSite.Url),
                    tags: new List<string> {"url", acuWebSite.Name},
                    timeout: timeout));
        }

        public static IHealthChecksBuilder AddSqlServers(
            this IHealthChecksBuilder builder,
            AcuWebSites acuWebSites,
            TimeSpan? timeout = null)
        {
            return acuWebSites.Aggregate(builder,
                (current, acuWebSite) => current.AddSqlServer(name: $"Sql [{acuWebSite.Name}]",
                    connectionString: acuWebSite.ConnectionString, 
                    tags: new List<string> {"sql", acuWebSite.Name},
                    timeout: timeout));
        }

        public static IHealthChecksBuilder AddActiveUsers(
            this IHealthChecksBuilder builder,
            AcuWebSites acuWebSites,
            TimeSpan? timeout = null)
        {
            return acuWebSites.Aggregate(builder,
                (current, acuWebSite) => current.AddCheck(name: $"Users [{acuWebSite.Name}]",
                    instance: new ActiveUsersChecker(acuWebSite),
                    tags: new List<string> {"users", acuWebSite.Name},
                    timeout: timeout));
        }

        public static IHealthChecksBuilder AddApplicationInsightsAvailabilityPublisher(this IHealthChecksBuilder builder)
        {
            builder.Services.AddSingleton((Func<IServiceProvider, IHealthCheckPublisher>) (sp => new ApplicationInsightsAvailabilityPublisher(sp.GetRequiredService<TelemetryClient>())));
            return builder;
        }

        public static HealthChecksUiSettings AddHealthCheckEndpoints(this HealthChecksUiSettings settings, AcuWebSites websites)
        {
            foreach (var website in websites ?? new AcuWebSites())
            {
                settings.AddHealthCheckEndpoint(website.Name, website.EndpointUrl);
            }

            return settings;
        }

        public static HealthChecksUiSettings AddWebhookNotifications(this HealthChecksUiSettings settings, Webhooks webhooks)
        {
            foreach (var webhook in webhooks ?? new Webhooks())
            {
                settings.AddWebhookNotification(webhook.Name, webhook.Url, webhook.FailurePayload,
                    webhook.RestorePayload, webhook.ShouldNotifyFunc, webhook.CustomMessageFunc,
                    webhook.CustomDescriptionFunc);
            }

            return settings;
        }
    }
}
