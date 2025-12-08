using BookingCalendar.Application.Abstractions;
using BookingCalendar.Infrastructure.Persistence;
using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookingCalendar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBookingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BookingCalendar")
                               ?? configuration.GetConnectionString("Default")
                               ?? "Server=sqlserver;Database=BookingCalendar;User=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddDbContext<BookingDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IBookingDbContext>(sp => sp.GetRequiredService<BookingDbContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork<BookingDbContext>>();

        return services;
    }
}
