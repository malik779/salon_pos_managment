using ClientService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace ClientService.Application.Clients;

public sealed record GetClientProfileByIdQuery(Guid Id) : IQuery<ClientProfileDto?>;

public sealed class GetClientProfileByIdQueryHandler : IRequestHandler<GetClientProfileByIdQuery, ClientProfileDto?>
{
    private readonly IClientRepository _repository;

    public GetClientProfileByIdQueryHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClientProfileDto?> Handle(GetClientProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetAsync(request.Id, cancellationToken);
        return profile is null
            ? null
            : new ClientProfileDto(profile.Id, profile.FirstName, profile.LastName, profile.Phone, profile.Email);
    }
}
