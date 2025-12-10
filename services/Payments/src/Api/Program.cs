using PaymentsService.Api;
using PaymentsService.Application;
using PaymentsService.Infrastructure;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("payments-service");

builder.Services
    .AddPaymentsApplication()
    .AddPaymentsInfrastructure();

var app = builder.Build();
app.UseServiceDefaults();
app.MapPaymentsEndpoints();
app.Run();
