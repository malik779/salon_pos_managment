using ClientService.Domain;

namespace ClientService.Application.Abstractions;

public interface IClientRepository
{
    Task<ClientProfile?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(ClientProfile profile, CancellationToken cancellationToken);
}
