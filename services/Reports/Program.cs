using System.Collections.Generic;
using ReportsService.Api;
using ReportsService.Infrastructure.Persistence;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("reports-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "reports-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddReportsInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapReportsEndpoints();
app.Run();
