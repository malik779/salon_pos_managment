using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Salon.ServiceDefaults.Messaging;

namespace Salon.ServiceDefaults;

public static class ServiceDefaultsExtensions
{
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder, string serviceName)
    {
        builder.Host.UseSerilog((context, cfg) => cfg
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.WithProperty("Service", serviceName)
            .WriteTo.Console());

        builder.Services.AddProblemDetails();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("default", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

        return builder;
    }

    public static WebApplication UseServiceDefaults(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("default");
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.MapHealthChecks("/health");
        return app;
    }
}
