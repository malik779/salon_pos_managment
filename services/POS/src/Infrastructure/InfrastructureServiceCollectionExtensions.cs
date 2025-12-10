using Microsoft.Extensions.DependencyInjection;
using PosService.Application.Abstractions;
using PosService.Infrastructure.Persistence;

namespace PosService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddPosInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IInvoiceRepository, InMemoryInvoiceRepository>();
        return services;
    }
}
