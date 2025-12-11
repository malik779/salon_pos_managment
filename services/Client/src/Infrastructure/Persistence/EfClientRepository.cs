using ClientService.Application.Abstractions;
using ClientService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClientService.Infrastructure.Persistence;

public sealed class EfClientRepository : IClientRepository
{
    private readonly ClientDbContext _dbContext;

    public EfClientRepository(ClientDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        await _dbContext.Clients.AddAsync(client, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Client?> GetAsync(Guid id, CancellationToken cancellationToken)
        => _dbContext.Clients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Client>> ListAsync(CancellationToken cancellationToken)
        => await _dbContext.Clients.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddConsentAsync(Guid clientId, ClientConsent consent, CancellationToken cancellationToken)
    {
        _dbContext.ClientConsents.Add(new ClientConsentEntity
        {
            ClientId = clientId,
            Version = consent.Version,
            CapturedAtUtc = consent.CapturedAtUtc
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<ClientConsent>> GetConsentsAsync(Guid clientId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.ClientConsents.AsNoTracking().Where(x => x.ClientId == clientId).ToListAsync(cancellationToken);
        return entities.Select(x => new ClientConsent(x.Version, x.CapturedAtUtc));
    }
}
