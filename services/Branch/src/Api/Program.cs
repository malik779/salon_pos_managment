using BranchService.Api;
using BranchService.Application;
using BranchService.Infrastructure;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("branch-service");

builder.Services
    .AddBranchApplication()
    .AddBranchInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapBranchEndpoints();
app.Run();
