using Salon.ServiceDefaults;
using StaffService.Api;
using StaffService.Application;
using StaffService.Infrastructure;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("staff-service");

builder.Services
    .AddStaffApplication()
    .AddStaffInfrastructure();

var app = builder.Build();
app.UseServiceDefaults();
app.MapStaffEndpoints();
app.Run();
