using Salon.ServiceDefaults;
using SyncService.Api;
using SyncService.Application;
using SyncService.Infrastructure;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("sync-service");

builder.Services
    .AddSyncApplication()
    .AddSyncInfrastructure();

var app = builder.Build();
app.UseServiceDefaults();
app.MapSyncEndpoints();
app.Run();
