using IdentityService.Application.Abstractions;
using IdentityService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.Persistence;

public static class IdentityDbSeeder
{
    public static async Task SeedAsync(IdentityDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        // Check if users already exist
        if (await context.Users.AnyAsync())
        {
            logger.LogInformation("Users already exist. Skipping seeding.");
            return;
        }

        // Create demo branches (we'll use fixed GUIDs that match Branch service seeding)
        var mainBranchId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var secondBranchId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // Default password for all demo users: Password123!
        var defaultPassword = "Password123!";
        var passwordHash = passwordHasher.HashPassword(defaultPassword);

        var users = new List<UserAccount>
        {
            // Admin user
            new UserAccount(
                id: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                email: "admin@salon.com",
                fullName: "Admin User",
                branchId: mainBranchId,
                role: "Admin",
                passwordHash: passwordHash
            ),
            // Manager user
            new UserAccount(
                id: Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                email: "manager@salon.com",
                fullName: "Manager User",
                branchId: mainBranchId,
                role: "Manager",
                passwordHash: passwordHash
            ),
            // Staff user 1
            new UserAccount(
                id: Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                email: "staff1@salon.com",
                fullName: "John Doe",
                branchId: mainBranchId,
                role: "Staff",
                passwordHash: passwordHash
            ),
            // Staff user 2
            new UserAccount(
                id: Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                email: "staff2@salon.com",
                fullName: "Jane Smith",
                branchId: mainBranchId,
                role: "Staff",
                passwordHash: passwordHash
            ),
            // Staff user 3 (second branch)
            new UserAccount(
                id: Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                email: "staff3@salon.com",
                fullName: "Bob Johnson",
                branchId: secondBranchId,
                role: "Staff",
                passwordHash: passwordHash
            )
        };

        await context.Users.AddRangeAsync(users);
        logger.LogInformation("Seeded {Count} users", users.Count);
    }
}
