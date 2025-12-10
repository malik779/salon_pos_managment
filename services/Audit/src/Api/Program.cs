using AuditService.Api;
using AuditService.Application;
using AuditService.Infrastructure;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("audit-service");

builder.Services
    .AddAuditApplication()
    .AddAuditInfrastructure();

var app = builder.Build();
app.UseServiceDefaults();
app.MapAuditEndpoints();
app.Run();
