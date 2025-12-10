using IdentityService.Application.Abstractions;
using IdentityService.Application.Users.Models;
using MediatR;

namespace IdentityService.Application.Users.Queries.GetUser;

public sealed record GetUserQuery(Guid Id) : IRequest<UserDto?>;

public sealed class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto?>
{
    private readonly IUserRepository _repository;

    public GetUserQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetAsync(request.Id, cancellationToken);
        return user is null ? null : new UserDto(user.Id, user.Email, user.FullName, user.Roles);
    }
}
