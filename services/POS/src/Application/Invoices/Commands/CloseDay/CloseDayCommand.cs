using PosService.Application.Abstractions;
using PosService.Application.Invoices.Models;
using FluentValidation;
using MediatR;

namespace PosService.Application.Invoices.Commands.CloseDay;

public sealed record CloseDayCommand(Guid BranchId, DateOnly BusinessDate) : IRequest<CloseDaySummary>;

public sealed class CloseDayCommandValidator : AbstractValidator<CloseDayCommand>
{
    public CloseDayCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
    }
}

public sealed class CloseDayCommandHandler : IRequestHandler<CloseDayCommand, CloseDaySummary>
{
    private readonly IInvoiceRepository _repository;

    public CloseDayCommandHandler(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<CloseDaySummary> Handle(CloseDayCommand request, CancellationToken cancellationToken)
    {
        var invoices = await _repository.ListAsync(request.BranchId, cancellationToken);
        var total = invoices.Sum(x => x.Total);
        var cash = total * 0.4m;
        var card = total - cash;
        var refunds = invoices.Where(x => x.Status == "Refunded").Sum(x => x.Total);
        return new CloseDaySummary(request.BranchId, request.BusinessDate, total, cash, card, refunds);
    }
}
