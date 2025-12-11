using System.Collections.Generic;
using NotificationsService.Api;
using NotificationsService.Application.Consumers;
using NotificationsService.Infrastructure.Persistence;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.Shared.Contracts.Events;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("notifications-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "notifications-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddNotificationsInfrastructure(builder.Configuration)
    .AddRabbitConsumer<InvoicePaidIntegrationEvent, InvoicePaidIntegrationEventHandler>();

var app = builder.Build();
app.UseServiceDefaults();
app.MapNotificationsEndpoints();
app.Run();
