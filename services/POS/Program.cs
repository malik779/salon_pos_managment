using System.Collections.Generic;
using PosService.Api;
using PosService.Infrastructure.Persistence;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("pos-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "pos-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddPosInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapPosEndpoints();
app.Run();
