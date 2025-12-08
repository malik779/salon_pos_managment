using ClientManagement.Domain.Clients;
using Microsoft.EntityFrameworkCore;

namespace ClientManagement.Application.Abstractions;

public interface IClientDbContext
{
    DbSet<ClientProfile> Clients { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
