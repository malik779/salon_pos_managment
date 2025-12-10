using ClientService.Application.Abstractions;
using ClientService.Application.Clients.Models;
using ClientService.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClientService.Application.Clients.Commands.CreateClient;

public sealed record CreateClientCommand(string FirstName, string LastName, string Phone, string? Email) : IRequest<ClientDto>;

public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Phone).NotEmpty();
    }
}

public sealed class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly IClientRepository _repository;

    public CreateClientCommandHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var client = new Client(Guid.NewGuid(), request.FirstName, request.LastName, request.Phone, request.Email);
        await _repository.AddAsync(client, cancellationToken);
        return new ClientDto(client.Id, client.FirstName, client.LastName, client.Phone, client.Email);
    }
}
