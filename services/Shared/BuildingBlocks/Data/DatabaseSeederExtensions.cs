using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Salon.BuildingBlocks.Data;

public static class DatabaseSeederExtensions
{
    /// <summary>
    /// Seeds the database if it's empty or in development environment.
    /// </summary>
    public static async Task SeedDatabaseAsync<TContext>(
        this IHost app,
        Func<TContext, Task> seedAction,
        bool forceSeed = false) where TContext : ServiceDbContextBase
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();
        var environment = services.GetRequiredService<IHostEnvironment>();

        try
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if database is empty or if we should force seed
            var hasData = await HasAnyDataAsync(context);

            if (!hasData || forceSeed || environment.IsDevelopment())
            {
                logger.LogInformation("Starting database seeding for {ContextType}", typeof(TContext).Name);
                await seedAction(context);
                await context.SaveChangesAsync();
                logger.LogInformation("Database seeding completed for {ContextType}", typeof(TContext).Name);
            }
            else
            {
                logger.LogInformation("Database already contains data. Skipping seeding for {ContextType}", typeof(TContext).Name);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database for {ContextType}", typeof(TContext).Name);
            throw;
        }
    }

    private static async Task<bool> HasAnyDataAsync<TContext>(TContext context) where TContext : ServiceDbContextBase
    {
        // Check if database can connect and has been migrated
        // The actual data existence check is done in each seeder
        try
        {
            return await context.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }
}
