using ClientService.Application.Abstractions;
using ClientService.Application.Clients.Models;
using MediatR;

namespace ClientService.Application.Clients.Queries.ListClients;

public sealed record ListClientsQuery() : IRequest<IReadOnlyList<ClientDto>>;

public sealed class ListClientsQueryHandler : IRequestHandler<ListClientsQuery, IReadOnlyList<ClientDto>>
{
    private readonly IClientRepository _repository;

    public ListClientsQueryHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ClientDto>> Handle(ListClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await _repository.ListAsync(cancellationToken);
        return clients.Select(c => new ClientDto(c.Id, c.FirstName, c.LastName, c.Phone, c.Email)).ToList();
    }
}
