using ClientService.Application.Abstractions;
using ClientService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Data;

namespace ClientService.Infrastructure.Persistence;

public sealed class ClientDbContext : ServiceDbContextBase
{
    public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options)
    {
    }

    public DbSet<ClientProfile> Clients => Set<ClientProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClientProfile>(builder =>
        {
            builder.ToTable("Clients");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        });
    }
}

internal sealed class ClientRepository : IClientRepository
{
    private readonly ClientDbContext _context;

    public ClientRepository(ClientDbContext context)
    {
        _context = context;
    }

    public async Task<ClientProfile?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Clients.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task AddAsync(ClientProfile profile, CancellationToken cancellationToken)
    {
        await _context.Clients.AddAsync(profile, cancellationToken);
    }

    public async Task<List<ClientProfile>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var query = _context.Clients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c =>
                c.FirstName.Contains(searchTerm) ||
                c.LastName.Contains(searchTerm) ||
                c.Email.Contains(searchTerm) ||
                c.Phone.Contains(searchTerm));
        }

        return await query
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync(cancellationToken);
    }
}

public static class ClientInfrastructureRegistration
{
    public static IServiceCollection AddClientInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<ClientDbContext>(configuration, "ClientDb");
        services.AddScoped<IClientRepository, ClientRepository>();
        return services;
    }
}
