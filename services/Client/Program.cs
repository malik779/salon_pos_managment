using System.Collections.Generic;
using ClientService.Api;
using ClientService.Infrastructure.Persistence;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("client-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "client-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddClientInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapClientEndpoints();
app.Run();
