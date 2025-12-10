using BookingService.Application.Abstractions;
using BookingService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddBookingInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
        services.AddSingleton<IIdempotencyStore, InMemoryIdempotencyStore>();
        services.AddSingleton<IAvailabilityService, InMemoryAvailabilityService>();
        return services;
    }
}
