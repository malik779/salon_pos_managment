using IdentityService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace IdentityService.Application.Users;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<UserDto?>;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _repository;

    public GetUserByIdQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetAsync(request.Id, cancellationToken);
        return user is null
            ? null
            : new UserDto(user.Id, user.Email, user.FullName, user.BranchId, user.Role, user.IsActive);
    }
}
