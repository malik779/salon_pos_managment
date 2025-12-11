using PosService.Api;
using PosService.Application;
using PosService.Infrastructure;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("pos-service");

builder.Services
    .AddPosApplication()
    .AddPosInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapPosEndpoints();
app.Run();
