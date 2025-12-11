using System.Collections.Generic;
using FluentValidation;
using MediatR;
using PaymentsService.Application.Abstractions;
using PaymentsService.Domain;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace PaymentsService.Application.PaymentIntents;

public sealed record PaymentIntentDto(Guid Id, Guid InvoiceId, decimal Amount, string Provider, string Status, DateTime CreatedAtUtc);

public sealed record CreatePaymentIntentCommand(Guid InvoiceId, decimal Amount, string Provider, string Actor)
    : ICommand<PaymentIntentDto>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "payments-service",
            Action: "PaymentIntentCreated",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["invoiceId"] = InvoiceId,
                ["amount"] = Amount,
                ["provider"] = Provider
            });
    }
}

public sealed class CreatePaymentIntentCommandValidator : AbstractValidator<CreatePaymentIntentCommand>
{
    public CreatePaymentIntentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Provider).NotEmpty();
    }
}

public sealed class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand, PaymentIntentDto>
{
    private readonly IPaymentIntentRepository _repository;

    public CreatePaymentIntentCommandHandler(IPaymentIntentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaymentIntentDto> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
    {
        var intent = new PaymentIntent(Guid.NewGuid(), request.InvoiceId, request.Amount, request.Provider);
        intent.MarkAuthorized();
        await _repository.AddAsync(intent, cancellationToken);

        return new PaymentIntentDto(intent.Id, intent.InvoiceId, intent.Amount, intent.Provider, intent.Status, intent.CreatedAtUtc);
    }
}
