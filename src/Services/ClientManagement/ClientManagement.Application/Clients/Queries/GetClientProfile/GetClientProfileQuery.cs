using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using ClientManagement.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ClientManagement.Application.Clients.Queries.GetClientProfile;

public sealed record ClientProfileDto(Guid Id, string Name, string Email, string Phone, DateOnly? DateOfBirth);

public sealed record GetClientProfileQuery(Guid ClientId) : IQuery<ClientProfileDto>;

public sealed class GetClientProfileQueryHandler(IClientDbContext context)
    : IQueryHandler<GetClientProfileQuery, ClientProfileDto>
{
    public async Task<Result<ClientProfileDto>> Handle(GetClientProfileQuery request, CancellationToken cancellationToken)
    {
        var client = await context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.ClientId, cancellationToken);

        if (client is null)
        {
            return Result.Failure<ClientProfileDto>(new Error("ClientManagement.ClientNotFound", "Client not found"));
        }

        var dto = new ClientProfileDto(client.Id, client.Name, client.Email, client.Phone, client.DateOfBirth);
        return Result.Success(dto);
    }
}
