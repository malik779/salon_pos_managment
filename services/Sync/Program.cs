using System.Collections.Generic;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;
using SyncService.Api;
using SyncService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("sync-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "sync-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddSyncInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapSyncEndpoints();
app.Run();
