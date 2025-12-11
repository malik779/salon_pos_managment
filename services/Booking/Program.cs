using System.Collections.Generic;
using BookingService.Api;
using BookingService.Infrastructure.Persistence;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("booking-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "booking-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddBookingInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapBookingEndpoints();
app.Run();
