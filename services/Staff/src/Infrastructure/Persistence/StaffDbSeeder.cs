using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffService.Domain;

namespace StaffService.Infrastructure.Persistence;

public static class StaffDbSeeder
{
    public static async Task SeedAsync(StaffDbContext context, ILogger logger)
    {
        // Check if staff members already exist
        if (await context.StaffMembers.AnyAsync())
        {
            logger.LogInformation("Staff members already exist. Skipping seeding.");
            return;
        }

        // Use the same user IDs from Identity service seeding
        var mainBranchId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var secondBranchId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var staffMembers = new List<StaffMember>
        {
            // Staff member 1 (John Doe)
            new StaffMember(
                id: Guid.Parse("c1111111-1111-1111-1111-111111111111"),
                userId: Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), // Matches Identity user
                defaultBranchId: mainBranchId,
                role: "Hair Stylist"
            ),
            // Staff member 2 (Jane Smith)
            new StaffMember(
                id: Guid.Parse("c2222222-2222-2222-2222-222222222222"),
                userId: Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), // Matches Identity user
                defaultBranchId: mainBranchId,
                role: "Senior Stylist"
            ),
            // Staff member 3 (Bob Johnson)
            new StaffMember(
                id: Guid.Parse("c3333333-3333-3333-3333-333333333333"),
                userId: Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), // Matches Identity user
                defaultBranchId: secondBranchId,
                role: "Color Specialist"
            )
        };

        await context.StaffMembers.AddRangeAsync(staffMembers);
        logger.LogInformation("Seeded {Count} staff members", staffMembers.Count);
    }
}
