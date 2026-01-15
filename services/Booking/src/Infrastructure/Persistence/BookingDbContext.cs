using BookingService.Application.Abstractions;
using BookingService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Data;

namespace BookingService.Infrastructure.Persistence;

public sealed class BookingDbContext : ServiceDbContextBase
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(builder =>
        {
            builder.ToTable("Appointments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Source).HasMaxLength(50).IsRequired();
            builder.HasIndex(x => new { x.BranchId, x.StaffId, x.StartUtc });
        });
    }
}

internal sealed class AppointmentRepository : IAppointmentRepository
{
    private readonly BookingDbContext _context;

    public AppointmentRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Appointments.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        await _context.Appointments.AddAsync(appointment, cancellationToken);
    }

    public async Task<List<Appointment>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var query = _context.Appointments.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a => a.Status.Contains(searchTerm) || a.Source.Contains(searchTerm));
        }

        return await query
            .OrderByDescending(a => a.StartUtc)
            .ToListAsync(cancellationToken);
    }
}

public static class BookingInfrastructureRegistration
{
    public static IServiceCollection AddBookingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<BookingDbContext>(configuration, "BookingDb");
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        return services;
    }
}
