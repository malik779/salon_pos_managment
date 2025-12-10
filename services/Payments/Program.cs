using FluentValidation;
using MediatR;
using Salon.ServiceDefaults;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("payments-service");

builder.Services.AddSingleton<PaymentIntentStore>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseServiceDefaults();

app.MapPost("/payments/charge", async (ChargePaymentCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));
app.MapPost("/payments/tokenize", (TokenizeCardRequest request) =>
{
    var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    return Results.Ok(new { token, brand = request.Brand, last4 = request.CardNumber[^4..] });
});

app.MapPost("/payments/webhook", async (HttpRequest httpRequest, PaymentIntentStore store) =>
{
    using var reader = new StreamReader(httpRequest.Body);
    var payload = await reader.ReadToEndAsync();
    var signature = httpRequest.Headers["X-Signature"].FirstOrDefault();
    if (!HmacValidator.IsValid(signature, payload, "secret"))
    {
        return Results.StatusCode(StatusCodes.Status400BadRequest);
    }

    store.MarkCompleted(Guid.NewGuid());
    return Results.Ok();
});

app.Run();

record ChargePaymentCommand(Guid InvoiceId, decimal Amount, string Method, string Provider) : IRequest<PaymentIntentDto>;
record PaymentIntentDto(Guid Id, Guid InvoiceId, decimal Amount, string Status, string ProviderReference);
record TokenizeCardRequest(string CardNumber, string Expiry, string Brand);

class ChargePaymentCommandValidator : AbstractValidator<ChargePaymentCommand>
{
    public ChargePaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).NotEmpty();
    }
}

class ChargePaymentHandler : IRequestHandler<ChargePaymentCommand, PaymentIntentDto>
{
    private readonly PaymentIntentStore _store;

    public ChargePaymentHandler(PaymentIntentStore store)
    {
        _store = store;
    }

    public Task<PaymentIntentDto> Handle(ChargePaymentCommand request, CancellationToken cancellationToken)
    {
        var intent = new PaymentIntentDto(Guid.NewGuid(), request.InvoiceId, request.Amount, "Authorized", Guid.NewGuid().ToString("N"));
        _store.Add(intent);
        return Task.FromResult(intent);
    }
}

class PaymentIntentStore
{
    private readonly Dictionary<Guid, PaymentIntentDto> _intents = new();

    public void Add(PaymentIntentDto dto) => _intents[dto.Id] = dto;
    public void MarkCompleted(Guid id)
    {
        if (_intents.TryGetValue(id, out var intent))
        {
            _intents[id] = intent with { Status = "Captured" };
        }
    }
}

static class HmacValidator
{
    public static bool IsValid(string? signature, string payload, string secret)
    {
        if (string.IsNullOrWhiteSpace(signature))
        {
            return false;
        }

        var expected = ComputeHash(payload, secret);
        return CryptographicOperations.FixedTimeEquals(Convert.FromHexString(signature), Convert.FromHexString(expected));
    }

    private static string ComputeHash(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash);
    }
}
