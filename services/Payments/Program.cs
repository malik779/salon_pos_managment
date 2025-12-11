using System.Collections.Generic;
using PaymentsService.Api;
using PaymentsService.Infrastructure.Persistence;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("payments-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "payments-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddPaymentsInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapPaymentsEndpoints();
app.Run();
