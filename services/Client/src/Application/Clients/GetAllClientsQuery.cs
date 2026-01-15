using ClientService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace ClientService.Application.Clients;

/// <summary>
/// Query to fetch all clients without pagination
/// Optimized for dropdowns, selects, and other UI components that need complete lists
/// Performance: Uses AsNoTracking for read-only queries to reduce memory overhead
/// </summary>
public sealed record GetAllClientsQuery(string? SearchTerm = null) : IQuery<List<ClientProfileDto>>;

public sealed class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, List<ClientProfileDto>>
{
    private readonly IClientRepository _repository;

    public GetAllClientsQueryHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ClientProfileDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await _repository.GetAllAsync(request.SearchTerm, cancellationToken);
        return clients.Select(c => new ClientProfileDto(c.Id, c.FirstName, c.LastName, c.Phone, c.Email)).ToList();
    }
}
