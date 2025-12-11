using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentsService.Application.Abstractions;
using PaymentsService.Infrastructure.Persistence;
using Salon.BuildingBlocks.Extensions;

namespace PaymentsService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<PaymentsDbContext>(configuration);
        services.AddScoped<IPaymentIntentRepository, EfPaymentIntentRepository>();
        return services;
    }
}
