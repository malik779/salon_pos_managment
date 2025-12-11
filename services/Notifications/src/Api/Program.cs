using NotificationsService.Api;
using NotificationsService.Application;
using NotificationsService.Application.Notifications.Consumers;
using NotificationsService.Infrastructure;
using Salon.BuildingBlocks.Messaging;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("notifications-service");

builder.Services
    .AddNotificationsApplication()
    .AddNotificationsInfrastructure(builder.Configuration)
    .AddRabbitConsumer<InvoicePaidMessageHandler, InvoicePaidMessage>(new RabbitMqConsumerDefinition("pos.invoices", "notifications.invoice-paid", "pos.invoice.created"));

var app = builder.Build();
app.UseServiceDefaults();
app.MapNotificationEndpoints();
app.Run();
