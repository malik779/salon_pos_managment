using System.Collections.Generic;
using BranchService.Api;
using BranchService.Infrastructure.Persistence;
using Salon.BuildingBlocks.Data;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("branch-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "branch-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddBranchInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();

// Seed database
await app.SeedDatabaseAsync<BranchDbContext>(async (context) =>
{
    var logger = app.Services.GetRequiredService<ILogger<BranchDbContext>>();
    await BranchDbSeeder.SeedAsync(context, logger);
});

app.MapBranchEndpoints();
app.Run();
