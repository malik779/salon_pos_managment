using ClientService.Application.Abstractions;
using ClientService.Application.Clients.Models;
using ClientService.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClientService.Application.Clients.Commands.CaptureConsent;

public sealed record CaptureConsentCommand(Guid ClientId, string Version) : IRequest<ConsentDto>;

public sealed class CaptureConsentCommandValidator : AbstractValidator<CaptureConsentCommand>
{
    public CaptureConsentCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
    }
}

public sealed class CaptureConsentCommandHandler : IRequestHandler<CaptureConsentCommand, ConsentDto>
{
    private readonly IClientRepository _repository;

    public CaptureConsentCommandHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<ConsentDto> Handle(CaptureConsentCommand request, CancellationToken cancellationToken)
    {
        var consent = new ClientConsent(request.Version, DateTime.UtcNow);
        await _repository.AddConsentAsync(request.ClientId, consent, cancellationToken);
        return new ConsentDto(request.ClientId, consent.Version, consent.CapturedAtUtc);
    }
}
