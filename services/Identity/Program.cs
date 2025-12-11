using System.Collections.Generic;
using IdentityService.Api;
using IdentityService.Infrastructure.Persistence;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("identity-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "identity-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddIdentityInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapIdentityEndpoints();
app.Run();
