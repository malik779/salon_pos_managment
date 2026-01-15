using CatalogService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CatalogService.Infrastructure.Persistence;

public static class CatalogDbSeeder
{
    public static async Task SeedAsync(CatalogDbContext context, ILogger logger)
    {
        // Check if catalog items already exist
        if (await context.CatalogItems.AnyAsync())
        {
            logger.LogInformation("Catalog items already exist. Skipping seeding.");
            return;
        }

        var catalogItems = new List<CatalogItem>
        {
            // Services
            new CatalogItem(
                id: Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                name: "Haircut - Men",
                type: "Service",
                price: 35.00m,
                durationMinutes: 30
            ),
            new CatalogItem(
                id: Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                name: "Haircut - Women",
                type: "Service",
                price: 55.00m,
                durationMinutes: 45
            ),
            new CatalogItem(
                id: Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                name: "Hair Color",
                type: "Service",
                price: 120.00m,
                durationMinutes: 120
            ),
            new CatalogItem(
                id: Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                name: "Hair Highlights",
                type: "Service",
                price: 150.00m,
                durationMinutes: 180
            ),
            new CatalogItem(
                id: Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                name: "Hair Styling",
                type: "Service",
                price: 45.00m,
                durationMinutes: 30
            ),
            new CatalogItem(
                id: Guid.Parse("a6666666-6666-6666-6666-666666666666"),
                name: "Hair Treatment",
                type: "Service",
                price: 80.00m,
                durationMinutes: 60
            ),
            new CatalogItem(
                id: Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                name: "Manicure",
                type: "Service",
                price: 25.00m,
                durationMinutes: 30
            ),
            new CatalogItem(
                id: Guid.Parse("a8888888-8888-8888-8888-888888888888"),
                name: "Pedicure",
                type: "Service",
                price: 35.00m,
                durationMinutes: 45
            ),
            new CatalogItem(
                id: Guid.Parse("a9999999-9999-9999-9999-999999999999"),
                name: "Facial",
                type: "Service",
                price: 75.00m,
                durationMinutes: 60
            ),
            new CatalogItem(
                id: Guid.Parse("b1111111-1111-1111-1111-111111111111"),
                name: "Massage",
                type: "Service",
                price: 90.00m,
                durationMinutes: 60
            ),
            // Products
            new CatalogItem(
                id: Guid.Parse("b2222222-2222-2222-2222-222222222222"),
                name: "Shampoo - Premium",
                type: "Product",
                price: 18.00m,
                durationMinutes: 0
            ),
            new CatalogItem(
                id: Guid.Parse("b3333333-3333-3333-3333-333333333333"),
                name: "Conditioner - Premium",
                type: "Product",
                price: 20.00m,
                durationMinutes: 0
            ),
            new CatalogItem(
                id: Guid.Parse("b4444444-4444-4444-4444-444444444444"),
                name: "Hair Styling Gel",
                type: "Product",
                price: 15.00m,
                durationMinutes: 0
            ),
            new CatalogItem(
                id: Guid.Parse("b5555555-5555-5555-5555-555555555555"),
                name: "Hair Spray",
                type: "Product",
                price: 12.00m,
                durationMinutes: 0
            ),
            new CatalogItem(
                id: Guid.Parse("b6666666-6666-6666-6666-666666666666"),
                name: "Nail Polish",
                type: "Product",
                price: 8.00m,
                durationMinutes: 0
            )
        };

        await context.CatalogItems.AddRangeAsync(catalogItems);
        logger.LogInformation("Seeded {Count} catalog items", catalogItems.Count);
    }
}
