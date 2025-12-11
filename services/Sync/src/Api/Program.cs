using Salon.ServiceDefaults;
using SyncService.Api;
using SyncService.Application;
using SyncService.Infrastructure;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("sync-service");

builder.Services
    .AddSyncApplication()
    .AddSyncInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapSyncEndpoints();
app.Run();
