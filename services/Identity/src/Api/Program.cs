using IdentityService.Api;
using IdentityService.Application;
using IdentityService.Infrastructure;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("identity-service");

builder.Services
    .AddIdentityApplication()
    .AddIdentityInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapIdentityEndpoints();
app.Run();
