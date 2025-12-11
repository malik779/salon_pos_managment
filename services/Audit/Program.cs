using System.Collections.Generic;
using AuditService.Api;
using AuditService.Infrastructure.Persistence;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("audit-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "audit-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddAuditInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapAuditEndpoints();
app.Run();
