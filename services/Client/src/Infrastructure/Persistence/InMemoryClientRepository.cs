using ClientService.Application.Abstractions;
using ClientService.Domain.Entities;

namespace ClientService.Infrastructure.Persistence;

public sealed class InMemoryClientRepository : IClientRepository
{
    private readonly Dictionary<Guid, Client> _clients = new();
    private readonly Dictionary<Guid, List<ClientConsent>> _consents = new();

    public Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        _clients[client.Id] = client;
        return Task.CompletedTask;
    }

    public Task<Client?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        _clients.TryGetValue(id, out var client);
        return Task.FromResult(client);
    }

    public Task<IReadOnlyList<Client>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Client> snapshot = _clients.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task AddConsentAsync(Guid clientId, ClientConsent consent, CancellationToken cancellationToken)
    {
        if (!_consents.TryGetValue(clientId, out var list))
        {
            list = new List<ClientConsent>();
            _consents[clientId] = list;
        }

        list.Add(consent);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<ClientConsent>> GetConsentsAsync(Guid clientId, CancellationToken cancellationToken)
    {
        if (!_consents.TryGetValue(clientId, out var list))
        {
            return Task.FromResult<IEnumerable<ClientConsent>>(Array.Empty<ClientConsent>());
        }

        return Task.FromResult<IEnumerable<ClientConsent>>(list.ToList());
    }
}
