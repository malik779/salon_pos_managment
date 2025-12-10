using CatalogService.Application.Abstractions;
using CatalogService.Application.Catalog.Models;
using MediatR;

namespace CatalogService.Application.Catalog.Queries.ListServices;

public sealed record ListServicesQuery() : IRequest<IReadOnlyList<ServiceDto>>;

public sealed class ListServicesQueryHandler : IRequestHandler<ListServicesQuery, IReadOnlyList<ServiceDto>>
{
    private readonly ICatalogRepository _repository;

    public ListServicesQueryHandler(ICatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ServiceDto>> Handle(ListServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _repository.ListServicesAsync(cancellationToken);
        return services.Select(s => new ServiceDto(s.Id, s.Name, s.DurationMinutes, s.BasePrice)).ToList();
    }
}
