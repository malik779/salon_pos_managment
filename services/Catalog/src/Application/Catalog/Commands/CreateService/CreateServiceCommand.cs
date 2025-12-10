using CatalogService.Application.Abstractions;
using CatalogService.Application.Catalog.Models;
using CatalogService.Domain.Entities;
using FluentValidation;
using MediatR;

namespace CatalogService.Application.Catalog.Commands.CreateService;

public sealed record CreateServiceCommand(string Name, int DurationMinutes, decimal BasePrice) : IRequest<ServiceDto>;

public sealed class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.BasePrice).GreaterThan(0);
    }
}

public sealed class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly ICatalogRepository _repository;

    public CreateServiceCommandHandler(ICatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = new ServiceItem(Guid.NewGuid(), request.Name, request.DurationMinutes, request.BasePrice);
        await _repository.AddServiceAsync(service, cancellationToken);
        return new ServiceDto(service.Id, service.Name, service.DurationMinutes, service.BasePrice);
    }
}
