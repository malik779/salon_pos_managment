using System.Collections.Generic;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;
using StaffService.Api;
using StaffService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("staff-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "staff-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddStaffInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapStaffEndpoints();
app.Run();
