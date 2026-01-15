using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Salon.ServiceDefaults;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("gateway-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "gateway-service"
});

// Configure Swagger for Gateway
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Salon POS Management - API Gateway",
        Version = "v1",
        Description = "API Gateway for Salon POS Management System. This gateway routes requests to various microservices.",
        Contact = new OpenApiContact
        {
            Name = "Salon POS Management",
            Email = "support@salonpos.com"
        }
    });
});

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add CORS for gateway
builder.Services.AddCors(options =>
{
    options.AddPolicy("gateway-cors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseServiceDefaults();
app.UseCors("gateway-cors");

// Enable Swagger in all environments (or remove this block to keep it Development-only)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Salon POS Management - API Gateway v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "API Gateway Documentation";
});

// Configuration endpoint for frontend runtime configuration
app.MapGet("/api/config", () =>
{
    var apiBaseUrl = builder.Configuration["Frontend:ApiBaseUrl"] 
        ?? Environment.GetEnvironmentVariable("FRONTEND_API_BASE_URL") 
        ?? "http://localhost:5000";
    
    var signalrHubUrl = builder.Configuration["Frontend:SignalrHubUrl"] 
        ?? Environment.GetEnvironmentVariable("FRONTEND_SIGNALR_HUB_URL") 
        ?? $"{apiBaseUrl}/hubs/updates";
    
    var environment = builder.Configuration["Frontend:Environment"] 
        ?? Environment.GetEnvironmentVariable("FRONTEND_ENVIRONMENT") 
        ?? builder.Environment.EnvironmentName;
    
    var version = builder.Configuration["Frontend:Version"] 
        ?? Environment.GetEnvironmentVariable("FRONTEND_VERSION") 
        ?? "1.0.0";

    return Results.Ok(new
    {
        apiBaseUrl,
        signalrHub = signalrHubUrl,
        environment,
        version,
        features = new Dictionary<string, bool>()
    });
})
.WithName("GetFrontendConfig")
.WithTags("Configuration")
.Produces(200)
.AllowAnonymous();

// Map reverse proxy routes
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.UseSessionAffinity();
    proxyPipeline.UseLoadBalancing();
    proxyPipeline.UsePassiveHealthChecks();
});

app.Run();

