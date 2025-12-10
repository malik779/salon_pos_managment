using ReportsService.Api;
using ReportsService.Application;
using ReportsService.Infrastructure;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("reports-service");

builder.Services
    .AddReportsApplication()
    .AddReportsInfrastructure();

var app = builder.Build();
app.UseServiceDefaults();
app.MapReportEndpoints();
app.Run();
