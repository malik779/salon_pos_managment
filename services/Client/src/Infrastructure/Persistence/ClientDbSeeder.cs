using ClientService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClientService.Infrastructure.Persistence;

public static class ClientDbSeeder
{
    public static async Task SeedAsync(ClientDbContext context, ILogger logger)
    {
        // Check if clients already exist
        if (await context.Clients.AnyAsync())
        {
            logger.LogInformation("Clients already exist. Skipping seeding.");
            return;
        }

        var clients = new List<ClientProfile>
        {
            new ClientProfile(
                id: Guid.Parse("d1111111-1111-1111-1111-111111111111"),
                firstName: "Alice",
                lastName: "Williams",
                phone: "+1-555-0101",
                email: "alice.williams@email.com"
            ),
            new ClientProfile(
                id: Guid.Parse("d2222222-2222-2222-2222-222222222222"),
                firstName: "Michael",
                lastName: "Brown",
                phone: "+1-555-0102",
                email: "michael.brown@email.com"
            ),
            new ClientProfile(
                id: Guid.Parse("d3333333-3333-3333-3333-333333333333"),
                firstName: "Sarah",
                lastName: "Davis",
                phone: "+1-555-0103",
                email: "sarah.davis@email.com"
            ),
            new ClientProfile(
                id: Guid.Parse("d4444444-4444-4444-4444-444444444444"),
                firstName: "David",
                lastName: "Miller",
                phone: "+1-555-0104",
                email: "david.miller@email.com"
            ),
            new ClientProfile(
                id: Guid.Parse("d5555555-5555-5555-5555-555555555555"),
                firstName: "Emily",
                lastName: "Wilson",
                phone: "+1-555-0105",
                email: "emily.wilson@email.com"
            ),
            new ClientProfile(
                id: Guid.Parse("d6666666-6666-6666-6666-666666666666"),
                firstName: "James",
                lastName: "Moore",
                phone: "+1-555-0106",
                email: "james.moore@email.com"
            ),
            new ClientProfile(
                id: Guid.Parse("d7777777-7777-7777-7777-777777777777"),
                firstName: "Olivia",
                lastName: "Taylor",
                phone: "+1-555-0107",
                email: "olivia.taylor@email.com"
            ),
            new ClientProfile(
                id: Guid.Parse("d8888888-8888-8888-8888-888888888888"),
                firstName: "Robert",
                lastName: "Anderson",
                phone: "+1-555-0108",
                email: "robert.anderson@email.com"
            )
        };

        await context.Clients.AddRangeAsync(clients);
        logger.LogInformation("Seeded {Count} clients", clients.Count);
    }
}
