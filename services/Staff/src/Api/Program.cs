using Salon.ServiceDefaults;
using StaffService.Api;
using StaffService.Application;
using StaffService.Infrastructure;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("staff-service");

builder.Services
    .AddStaffApplication()
    .AddStaffInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapStaffEndpoints();
app.Run();
