using ClientService.Application.Abstractions;
using ClientService.Application.Clients.Models;
using MediatR;

namespace ClientService.Application.Clients.Queries.ListConsents;

public sealed record ListConsentsQuery(Guid ClientId) : IRequest<IEnumerable<ConsentDto>>;

public sealed class ListConsentsQueryHandler : IRequestHandler<ListConsentsQuery, IEnumerable<ConsentDto>>
{
    private readonly IClientRepository _repository;

    public ListConsentsQueryHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ConsentDto>> Handle(ListConsentsQuery request, CancellationToken cancellationToken)
    {
        var consents = await _repository.GetConsentsAsync(request.ClientId, cancellationToken);
        return consents.Select(c => new ConsentDto(request.ClientId, c.Version, c.CapturedAtUtc));
    }
}
