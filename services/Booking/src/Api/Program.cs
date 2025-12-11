using BookingService.Api;
using BookingService.Application;
using BookingService.Infrastructure;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("booking-service");

builder.Services
    .AddBookingApplication()
    .AddBookingInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapBookingEndpoints();
app.Run();
