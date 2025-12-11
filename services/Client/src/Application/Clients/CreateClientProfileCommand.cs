using System.Collections.Generic;
using ClientService.Application.Abstractions;
using ClientService.Domain;
using FluentValidation;
using MediatR;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace ClientService.Application.Clients;

public sealed record ClientProfileDto(Guid Id, string FirstName, string LastName, string Phone, string Email);

public sealed record CreateClientProfileCommand(string FirstName, string LastName, string Phone, string Email, string Actor)
    : ICommand<ClientProfileDto>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "client-service",
            Action: "ClientCreated",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["phone"] = Phone,
                ["email"] = Email
            });
    }
}

public sealed class CreateClientProfileCommandValidator : AbstractValidator<CreateClientProfileCommand>
{
    public CreateClientProfileCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public sealed class CreateClientProfileCommandHandler : IRequestHandler<CreateClientProfileCommand, ClientProfileDto>
{
    private readonly IClientRepository _repository;

    public CreateClientProfileCommandHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClientProfileDto> Handle(CreateClientProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = new ClientProfile(Guid.NewGuid(), request.FirstName, request.LastName, request.Phone, request.Email);
        await _repository.AddAsync(profile, cancellationToken);
        return new ClientProfileDto(profile.Id, profile.FirstName, profile.LastName, profile.Phone, profile.Email);
    }
}
