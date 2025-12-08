using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BuildingBlocks.Infrastructure.Logging;

public static class SerilogExtensions
{
    public static IHostBuilder UseSalonSerilog(this IHostBuilder hostBuilder, string applicationName)
    {
        hostBuilder.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithEnvironmentName()
                .Enrich.WithProcessId()
                .WriteTo.Console();
        });

        return hostBuilder;
    }
}
