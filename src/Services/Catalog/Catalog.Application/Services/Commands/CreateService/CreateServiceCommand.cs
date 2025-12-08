using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Models;
using BuildingBlocks.Domain.Primitives;
using Catalog.Application.Abstractions;
using Catalog.Domain.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Services.Commands.CreateService;

public sealed record CreateServiceCommand(string Name, int DurationMinutes, decimal BasePrice, string Category) : ICommand<Guid>;

public sealed class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
    }
}

public sealed class CreateServiceCommandHandler(ICatalogDbContext context)
    : ICommandHandler<CreateServiceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.Services.AnyAsync(s => s.Name == request.Name, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(new Error("Catalog.ServiceExists", "A service with the same name already exists"));
        }

        var service = ServiceItem.Create(request.Name, request.DurationMinutes, request.BasePrice, request.Category);
        context.Services.Add(service);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(service.Id);
    }
}
