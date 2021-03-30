using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApp.Models;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public Settings Settings => Configuration.GetSection(nameof(Settings)).Get<Settings>();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(GetInstrumentationKey());

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = Constants.Title, Version = "v1" });
            });

            services.AddHealthChecks()
                .AddUrls(Settings.AcuWebSites, TimeSpan.FromSeconds(2))
                .AddSqlServers(Settings.AcuWebSites, TimeSpan.FromSeconds(2))
                .AddActiveUsers(Settings.AcuWebSites, TimeSpan.FromSeconds(2))
                .AddApplicationInsightsAvailabilityPublisher()
                .AddApplicationInsightsPublisher();

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(10);
                options.Period = TimeSpan.FromSeconds(30);
                options.Timeout = TimeSpan.FromSeconds(60);
            });

            services.AddHealthChecksUI(settings =>
            {
                settings.SetApiMaxActiveRequests(Constants.MaxHealthChecksRequests);
                settings.MaximumHistoryEntriesPerEndpoint(Constants.MaxHealthChecksEntries);
                settings.SetEvaluationTimeInSeconds(Constants.EvaluationTimeInSeconds);
                settings.SetMinimumSecondsBetweenFailureNotifications(Constants.NotificationTimeInSeconds);
                settings.AddHealthCheckEndpoints(Settings.AcuWebSites);
                settings.AddWebhookNotifications(Settings.Webhooks);
            }).AddInMemoryStorage();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Constants.Title} v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                foreach (var acuWebSite in Settings.AcuWebSites)
                {
                    endpoints.MapHealthChecks(acuWebSite.EndpointUrl, acuWebSite.Options);
                }
                endpoints.MapHealthChecksUI();
            });
        }

        private string GetInstrumentationKey()
        {
            const string key = @"Serilog:WriteTo:2:Args:instrumentationKey";
            var instrumentationKey = Configuration.GetValue<string>(key);
            return instrumentationKey;
        }
    }
}
