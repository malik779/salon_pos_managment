using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentsService.Application.Abstractions;
using PaymentsService.Domain;
using Salon.BuildingBlocks.Data;

namespace PaymentsService.Infrastructure.Persistence;

public sealed class PaymentsDbContext : ServiceDbContextBase
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options)
    {
    }

    public DbSet<PaymentIntent> PaymentIntents => Set<PaymentIntent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentIntent>(builder =>
        {
            builder.ToTable("PaymentIntents");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Provider).HasMaxLength(100).IsRequired();
        });
    }
}

internal sealed class PaymentIntentRepository : IPaymentIntentRepository
{
    private readonly PaymentsDbContext _context;

    public PaymentIntentRepository(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentIntent?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.PaymentIntents.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task AddAsync(PaymentIntent intent, CancellationToken cancellationToken)
    {
        await _context.PaymentIntents.AddAsync(intent, cancellationToken);
    }
}

public static class PaymentsInfrastructureRegistration
{
    public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<PaymentsDbContext>(configuration, "PaymentsDb");
        services.AddScoped<IPaymentIntentRepository, PaymentIntentRepository>();
        return services;
    }
}
