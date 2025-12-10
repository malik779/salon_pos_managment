using IdentityService.Application.Abstractions;
using IdentityService.Application.Users.Models;
using MediatR;

namespace IdentityService.Application.Users.Queries.ListUsers;

public sealed record ListUsersQuery() : IRequest<IReadOnlyList<UserDto>>;

public sealed class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserRepository _repository;

    public ListUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<UserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _repository.ListAsync(cancellationToken);
        return users.Select(user => new UserDto(user.Id, user.Email, user.FullName, user.Roles)).ToList();
    }
}
