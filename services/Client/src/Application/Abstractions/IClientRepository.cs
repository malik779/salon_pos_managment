using ClientService.Domain;

namespace ClientService.Application.Abstractions;

public interface IClientRepository
{
    Task<ClientProfile?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(ClientProfile profile, CancellationToken cancellationToken);
    /// <summary>
    /// Gets all clients without pagination
    /// Optimized for read-only queries using AsNoTracking
    /// </summary>
    Task<List<ClientProfile>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken);
}
