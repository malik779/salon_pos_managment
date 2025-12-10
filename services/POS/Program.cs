using FluentValidation;
using MediatR;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("pos-service");

builder.Services.AddSingleton<InvoiceStore>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseServiceDefaults();

var invoices = app.MapGroup("/invoices");

invoices.MapPost("", async (CreateInvoiceCommand command, IMediator mediator) =>
{
    var invoice = await mediator.Send(command);
    return Results.Created($"/invoices/{invoice.Id}", invoice);
});

invoices.MapPost("/{id:guid}/finalize", async (Guid id, IMediator mediator) =>
{
    var result = await mediator.Send(new FinalizeInvoiceCommand(id));
    return result is null ? Results.NotFound() : Results.Ok(result);
});

invoices.MapPost("/{id:guid}/refund", async (Guid id, RefundInvoiceCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command with { InvoiceId = id });
    return result is null ? Results.NotFound() : Results.Ok(result);
});

var payments = app.MapGroup("/payments");

payments.MapPost("", async (CapturePaymentCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));

app.MapPost("/closereads", async (CloseDayCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));

app.Run();

record InvoiceLineDto(Guid ItemId, string ItemType, int Quantity, decimal UnitPrice);
record InvoiceDto(Guid Id, Guid BranchId, Guid ClientId, decimal Subtotal, decimal Tax, decimal Discount, decimal Total, string Status, List<InvoiceLineDto> Lines);
record PaymentDto(Guid PaymentId, Guid InvoiceId, string Method, decimal Amount, string Status);
record CloseDaySummary(Guid BranchId, DateOnly BusinessDate, decimal TotalSales, decimal CashTotal, decimal CardTotal, decimal Refunds);

record CreateInvoiceCommand(Guid BranchId, Guid ClientId, List<InvoiceLineDto> Lines, decimal Discount) : IRequest<InvoiceDto>;
record FinalizeInvoiceCommand(Guid InvoiceId) : IRequest<InvoiceDto?>;
record RefundInvoiceCommand(Guid InvoiceId, decimal Amount, string Reason) : IRequest<InvoiceDto?>;
record CapturePaymentCommand(Guid InvoiceId, string Method, decimal Amount) : IRequest<PaymentDto>;
record CloseDayCommand(Guid BranchId, DateOnly BusinessDate) : IRequest<CloseDaySummary>;

class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Lines).NotEmpty();
    }
}

class CapturePaymentCommandValidator : AbstractValidator<CapturePaymentCommand>
{
    public CapturePaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Method).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

class InvoiceStore
{
    private readonly Dictionary<Guid, InvoiceDto> _invoices = new();

    public InvoiceDto Add(InvoiceDto dto)
    {
        _invoices[dto.Id] = dto;
        return dto;
    }

    public InvoiceDto? Get(Guid id) => _invoices.TryGetValue(id, out var dto) ? dto : null;
    public IEnumerable<InvoiceDto> All() => _invoices.Values;
}

class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
{
    private readonly InvoiceStore _store;

    public CreateInvoiceHandler(InvoiceStore store)
    {
        _store = store;
    }

    public Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var subtotal = request.Lines.Sum(x => x.UnitPrice * x.Quantity);
        var tax = subtotal * 0.12m;
        var total = subtotal + tax - request.Discount;
        var invoice = new InvoiceDto(Guid.NewGuid(), request.BranchId, request.ClientId, subtotal, tax, request.Discount, total, "Draft", request.Lines);
        _store.Add(invoice);
        return Task.FromResult(invoice);
    }
}

class FinalizeInvoiceHandler : IRequestHandler<FinalizeInvoiceCommand, InvoiceDto?>
{
    private readonly InvoiceStore _store;

    public FinalizeInvoiceHandler(InvoiceStore store)
    {
        _store = store;
    }

    public Task<InvoiceDto?> Handle(FinalizeInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = _store.Get(request.InvoiceId);
        if (invoice is null)
        {
            return Task.FromResult<InvoiceDto?>(null);
        }

        var finalized = invoice with { Status = "Finalized" };
        _store.Add(finalized);
        return Task.FromResult<InvoiceDto?>(finalized);
    }
}

class RefundInvoiceHandler : IRequestHandler<RefundInvoiceCommand, InvoiceDto?>
{
    private readonly InvoiceStore _store;

    public RefundInvoiceHandler(InvoiceStore store)
    {
        _store = store;
    }

    public Task<InvoiceDto?> Handle(RefundInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = _store.Get(request.InvoiceId);
        if (invoice is null)
        {
            return Task.FromResult<InvoiceDto?>(null);
        }

        var refunded = invoice with { Status = "Refunded" };
        _store.Add(refunded);
        return Task.FromResult<InvoiceDto?>(refunded);
    }
}

class CapturePaymentHandler : IRequestHandler<CapturePaymentCommand, PaymentDto>
{
    public Task<PaymentDto> Handle(CapturePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new PaymentDto(Guid.NewGuid(), request.InvoiceId, request.Method, request.Amount, "Captured");
        return Task.FromResult(payment);
    }
}

class CloseDayHandler : IRequestHandler<CloseDayCommand, CloseDaySummary>
{
    private readonly InvoiceStore _store;

    public CloseDayHandler(InvoiceStore store)
    {
        _store = store;
    }

    public Task<CloseDaySummary> Handle(CloseDayCommand request, CancellationToken cancellationToken)
    {
        var invoices = _store.All().Where(x => x.BranchId == request.BranchId).ToList();
        var total = invoices.Sum(x => x.Total);
        var cash = total * 0.4m;
        var card = total - cash;
        var refunds = invoices.Where(x => x.Status == \"Refunded\").Sum(x => x.Total);
        var summary = new CloseDaySummary(request.BranchId, request.BusinessDate, total, cash, card, refunds);
        return Task.FromResult(summary);
    }
}
