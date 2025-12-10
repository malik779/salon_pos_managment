using FluentValidation;
using IdentityService.Application.Abstractions;
using IdentityService.Application.Users.Models;
using IdentityService.Domain.Entities;
using MediatR;
using Salon.ServiceDefaults.Messaging;

namespace IdentityService.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(string Email, string FullName, string[] Roles) : IRequest<UserDto>;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(3);
    }
}

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public CreateUserCommandHandler(IUserRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User(Guid.NewGuid(), request.Email, request.FullName, request.Roles);
        await _repository.AddAsync(user, cancellationToken);

        await _eventPublisher.PublishAsync("identity.users", "identity.user.created", new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.Roles
        }, cancellationToken);

        return new UserDto(user.Id, user.Email, user.FullName, user.Roles);
    }
}
