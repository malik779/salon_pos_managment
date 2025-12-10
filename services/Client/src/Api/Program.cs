using ClientService.Api;
using ClientService.Application;
using ClientService.Infrastructure;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("client-service");

builder.Services
    .AddClientApplication()
    .AddClientInfrastructure();

var app = builder.Build();
app.UseServiceDefaults();
app.MapClientEndpoints();
app.Run();
