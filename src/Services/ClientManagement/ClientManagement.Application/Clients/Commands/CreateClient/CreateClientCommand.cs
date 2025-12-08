using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using ClientManagement.Application.Abstractions;
using ClientManagement.Domain.Clients;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ClientManagement.Application.Clients.Commands.CreateClient;

public sealed record CreateClientCommand(string Name, string Email, string Phone) : ICommand<Guid>;

public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(50);
    }
}

public sealed class CreateClientCommandHandler(IClientDbContext context)
    : ICommandHandler<CreateClientCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.Clients.AnyAsync(c => c.Email == request.Email, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(new Error("ClientManagement.Exists", "Client already exists"));
        }

        var client = ClientProfile.Create(request.Name, request.Email, request.Phone);
        context.Clients.Add(client);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(client.Id);
    }
}
