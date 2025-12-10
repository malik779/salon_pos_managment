using NotificationsService.Api;
using NotificationsService.Application;
using NotificationsService.Infrastructure;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("notifications-service");

builder.Services
    .AddNotificationsApplication()
    .AddNotificationsInfrastructure();

var app = builder.Build();
app.UseServiceDefaults();
app.MapNotificationEndpoints();
app.Run();
