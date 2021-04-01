using FunctionApp;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using FunctionApp.Models;
using FunctionApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

[assembly: FunctionsStartup(typeof(Startup))]
namespace FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = ConfigureAppSettings(builder);
            ConfigureAppLogging(builder, configuration);
            ConfigureAppIoc(builder, configuration);
        }

        private static IConfiguration ConfigureAppSettings(IFunctionsHostBuilder builder)
        {
            var context = builder.GetContext();
            var environment = context.EnvironmentName;
            var workingDirectory = context.ApplicationRootPath;
            var configuration = new ConfigurationBuilder()
                .SetBasePath(workingDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            return configuration;
        }

        private static void ConfigureAppLogging(IFunctionsHostBuilder builder, IConfiguration configuration)
        {
            var instrumentationKey = configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");

            builder.Services
                .AddLogging(loggingBuilder =>
                    loggingBuilder.AddSerilog(
                        new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .Enrich.FromLogContext()
                            .WriteTo.ApplicationInsights(instrumentationKey, TelemetryConverter.Traces)
                            .CreateLogger()));
        }

        private static void ConfigureAppIoc(IFunctionsHostBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddSingleton<ISender, SmsSender>();
            builder.Services.Configure<Settings>(configuration.GetSection(nameof(Settings)));
        }
    }
}
