using System.Collections.Generic;
using CatalogService.Api;
using CatalogService.Infrastructure.Persistence;
using Salon.BuildingBlocks.Data;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("catalog-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "catalog-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddCatalogInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();

// Seed database
await app.SeedDatabaseAsync<CatalogDbContext>(async (context) =>
{
    var logger = app.Services.GetRequiredService<ILogger<CatalogDbContext>>();
    await CatalogDbSeeder.SeedAsync(context, logger);
});

app.MapCatalogEndpoints();
app.Run();
