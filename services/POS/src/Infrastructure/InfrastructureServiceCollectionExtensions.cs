using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PosService.Application.Abstractions;
using PosService.Infrastructure.Persistence;
using Salon.BuildingBlocks.Extensions;

namespace PosService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddPosInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<PosDbContext>(configuration);
        services.AddScoped<IInvoiceRepository, EfInvoiceRepository>();
        return services;
    }
}
