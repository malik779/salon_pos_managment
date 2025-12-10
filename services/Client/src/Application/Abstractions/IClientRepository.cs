using ClientService.Domain.Entities;

namespace ClientService.Application.Abstractions;

public interface IClientRepository
{
    Task AddAsync(Client client, CancellationToken cancellationToken);
    Task<Client?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Client>> ListAsync(CancellationToken cancellationToken);
    Task AddConsentAsync(Guid clientId, ClientConsent consent, CancellationToken cancellationToken);
    Task<IEnumerable<ClientConsent>> GetConsentsAsync(Guid clientId, CancellationToken cancellationToken);
}
