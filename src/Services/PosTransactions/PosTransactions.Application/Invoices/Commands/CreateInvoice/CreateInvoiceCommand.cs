using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PosTransactions.Application.Abstractions;
using PosTransactions.Domain.Invoices;

namespace PosTransactions.Application.Invoices.Commands.CreateInvoice;

public sealed record CreateInvoiceCommand(Guid BranchId, Guid ClientId, decimal Amount, decimal Tax, decimal Discount) : ICommand<Guid>;

public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public sealed class CreateInvoiceCommandHandler(IPosDbContext context)
    : ICommandHandler<CreateInvoiceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = Invoice.Create(request.BranchId, request.ClientId, request.Amount, request.Tax, request.Discount);
        context.Invoices.Add(invoice);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(invoice.Id);
    }
}
