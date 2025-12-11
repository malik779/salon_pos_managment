using System.Collections.Generic;
using CatalogService.Application.Abstractions;
using CatalogService.Domain;
using FluentValidation;
using MediatR;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace CatalogService.Application.Items;

public sealed record CatalogItemDto(Guid Id, string Name, string Type, decimal Price, int DurationMinutes);

public sealed record DefineCatalogItemCommand(Guid? Id, string Name, string Type, decimal Price, int DurationMinutes, string Actor)
    : ICommand<CatalogItemDto>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "catalog-service",
            Action: "CatalogItemDefined",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["itemId"] = Id,
                ["name"] = Name,
                ["type"] = Type,
                ["price"] = Price
            });
    }
}

public sealed class DefineCatalogItemCommandValidator : AbstractValidator<DefineCatalogItemCommand>
{
    public DefineCatalogItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.DurationMinutes).GreaterThanOrEqualTo(0);
    }
}

public sealed class DefineCatalogItemCommandHandler : IRequestHandler<DefineCatalogItemCommand, CatalogItemDto>
{
    private readonly ICatalogRepository _repository;

    public DefineCatalogItemCommandHandler(ICatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<CatalogItemDto> Handle(DefineCatalogItemCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Id.HasValue
            ? await _repository.GetAsync(request.Id.Value, cancellationToken) ?? new CatalogItem(request.Id.Value, request.Name, request.Type, request.Price, request.DurationMinutes)
            : new CatalogItem(Guid.NewGuid(), request.Name, request.Type, request.Price, request.DurationMinutes);

        entity.Update(request.Name, request.Type, request.Price, request.DurationMinutes);
        await _repository.UpsertAsync(entity, cancellationToken);

        return new CatalogItemDto(entity.Id, entity.Name, entity.Type, entity.Price, entity.DurationMinutes);
    }
}
