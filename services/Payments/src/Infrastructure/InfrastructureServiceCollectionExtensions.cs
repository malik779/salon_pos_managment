using Microsoft.Extensions.DependencyInjection;
using PaymentsService.Application.Abstractions;
using PaymentsService.Infrastructure.Persistence;

namespace PaymentsService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentIntentRepository, InMemoryPaymentIntentRepository>();
        return services;
    }
}
