using FluentValidation;
using MediatR;
using PaymentsService.Application.Abstractions;
using PaymentsService.Application.Payments.Models;
using PaymentsService.Domain.Entities;
using Salon.ServiceDefaults.Messaging;

namespace PaymentsService.Application.Payments.Commands.ChargePayment;

public sealed record ChargePaymentCommand(Guid InvoiceId, decimal Amount, string Method, string Provider) : IRequest<PaymentIntentDto>;

public sealed class ChargePaymentCommandValidator : AbstractValidator<ChargePaymentCommand>
{
    public ChargePaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).NotEmpty();
    }
}

public sealed class ChargePaymentCommandHandler : IRequestHandler<ChargePaymentCommand, PaymentIntentDto>
{
    private readonly IPaymentIntentRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public ChargePaymentCommandHandler(IPaymentIntentRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<PaymentIntentDto> Handle(ChargePaymentCommand request, CancellationToken cancellationToken)
    {
        var intent = new PaymentIntent(Guid.NewGuid(), request.InvoiceId, request.Amount, request.Method, "Authorized", Guid.NewGuid().ToString("N"));
        await _repository.AddAsync(intent, cancellationToken);
        await _eventPublisher.PublishAsync("payments", "payments.authorized", new { intent.Id, intent.InvoiceId, intent.Amount }, cancellationToken);
        return new PaymentIntentDto(intent.Id, intent.InvoiceId, intent.Amount, intent.Status, intent.ProviderReference);
    }
}
