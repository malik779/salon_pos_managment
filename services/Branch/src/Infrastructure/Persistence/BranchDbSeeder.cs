using BranchService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BranchService.Infrastructure.Persistence;

public static class BranchDbSeeder
{
    public static async Task SeedAsync(BranchDbContext context, ILogger logger)
    {
        // Check if branches already exist
        if (await context.Branches.AnyAsync())
        {
            logger.LogInformation("Branches already exist. Skipping seeding.");
            return;
        }

        var branches = new List<Branch>
        {
            new Branch(
                id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
                name: "Main Street Salon",
                timezone: "America/New_York",
                address: "123 Main Street, New York, NY 10001"
            ),
            new Branch(
                id: Guid.Parse("22222222-2222-2222-2222-222222222222"),
                name: "Downtown Branch",
                timezone: "America/Chicago",
                address: "456 Downtown Ave, Chicago, IL 60601"
            ),
            new Branch(
                id: Guid.Parse("33333333-3333-3333-3333-333333333333"),
                name: "Westside Location",
                timezone: "America/Los_Angeles",
                address: "789 Westside Blvd, Los Angeles, CA 90001"
            )
        };

        await context.Branches.AddRangeAsync(branches);
        logger.LogInformation("Seeded {Count} branches", branches.Count);
    }
}
