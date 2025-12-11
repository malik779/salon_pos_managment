using CatalogService.Api;
using CatalogService.Application;
using CatalogService.Infrastructure;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("catalog-service");

builder.Services
    .AddCatalogApplication()
    .AddCatalogInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapCatalogEndpoints();
app.Run();
