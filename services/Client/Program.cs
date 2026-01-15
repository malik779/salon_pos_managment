using System.Collections.Generic;
using ClientService.Api;
using ClientService.Infrastructure.Persistence;
using Salon.BuildingBlocks.Data;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("client-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "client-service"
});

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddClientInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();

// Seed database
await app.SeedDatabaseAsync<ClientDbContext>(async (context) =>
{
    var logger = app.Services.GetRequiredService<ILogger<ClientDbContext>>();
    await ClientDbSeeder.SeedAsync(context, logger);
});

app.MapClientEndpoints();
app.Run();
