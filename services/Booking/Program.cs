using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("booking-service");

builder.Services.AddSingleton<BookingStore>();
builder.Services.AddSingleton<IdempotencyStore>();
builder.Services.AddSingleton<AvailabilityService>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseServiceDefaults();

var group = app.MapGroup("/bookings");

group.MapPost("", async ([FromBody] CreateBookingCommand command, [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey, IMediator mediator) =>
{
    if (string.IsNullOrWhiteSpace(idempotencyKey))
    {
        return Results.BadRequest(new { error = "missing_idempotency_key" });
    }

    command = command with { IdempotencyKey = idempotencyKey };
    var result = await mediator.Send(command);
    return Results.Created($"/bookings/{result.Id}", result);
});

group.MapGet("/{id:guid}", (Guid id, BookingStore store) =>
{
    var booking = store.Get(id);
    return booking is null ? Results.NotFound() : Results.Ok(booking);
});

group.MapPatch("/{id:guid}/status", async (Guid id, [FromBody] UpdateBookingStatusCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command with { BookingId = id });
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapGet("/calendar", (Guid branchId, DateTime from, DateTime to, BookingStore store) =>
{
    var bookings = store.List(branchId, from, to);
    return Results.Ok(bookings);
});

app.Run();

record BookingDto(Guid Id, Guid BranchId, Guid ClientId, Guid StaffId, DateTime StartUtc, DateTime EndUtc, string Status, string IdempotencyKey);

record CreateBookingCommand(Guid BranchId, Guid ClientId, Guid StaffId, DateTime StartUtc, DateTime EndUtc, string IdempotencyKey) : IRequest<BookingDto>;
record UpdateBookingStatusCommand(Guid BookingId, string Status) : IRequest<BookingDto?>;

class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.EndUtc).GreaterThan(x => x.StartUtc);
        RuleFor(x => x.IdempotencyKey).NotEmpty();
    }
}

class UpdateBookingStatusCommandValidator : AbstractValidator<UpdateBookingStatusCommand>
{
    public UpdateBookingStatusCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
    }
}

class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly BookingStore _store;
    private readonly IdempotencyStore _idempotency;
    private readonly AvailabilityService _availability;

    public CreateBookingHandler(BookingStore store, IdempotencyStore idempotency, AvailabilityService availability)
    {
        _store = store;
        _idempotency = idempotency;
        _availability = availability;
    }

    public Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        if (_idempotency.TryGet(request.IdempotencyKey, out var cached))
        {
            return Task.FromResult(cached);
        }

        if (!_availability.IsSlotAvailable(request.BranchId, request.StaffId, request.StartUtc, request.EndUtc))
        {
            throw new InvalidOperationException("slot_unavailable");
        }

        var booking = new BookingDto(Guid.NewGuid(), request.BranchId, request.ClientId, request.StaffId, request.StartUtc, request.EndUtc, "Confirmed", request.IdempotencyKey);
        _store.Add(booking);
        _idempotency.Remember(request.IdempotencyKey, booking);
        _availability.BlockSlot(request.BranchId, request.StaffId, request.StartUtc, request.EndUtc);
        return Task.FromResult(booking);
    }
}

class UpdateBookingStatusHandler : IRequestHandler<UpdateBookingStatusCommand, BookingDto?>
{
    private readonly BookingStore _store;

    public UpdateBookingStatusHandler(BookingStore store)
    {
        _store = store;
    }

    public Task<BookingDto?> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var booking = _store.Get(request.BookingId);
        if (booking is null)
        {
            return Task.FromResult<BookingDto?>(null);
        }

        var updated = booking with { Status = request.Status };
        _store.Add(updated);
        return Task.FromResult<BookingDto?>(updated);
    }
}

class BookingStore
{
    private readonly Dictionary<Guid, BookingDto> _bookings = new();

    public void Add(BookingDto dto) => _bookings[dto.Id] = dto;
    public BookingDto? Get(Guid id) => _bookings.TryGetValue(id, out var dto) ? dto : null;

    public IEnumerable<BookingDto> List(Guid branchId, DateTime from, DateTime to) => _bookings.Values
        .Where(x => x.BranchId == branchId && x.StartUtc >= from && x.EndUtc <= to)
        .OrderBy(x => x.StartUtc);
}

class IdempotencyStore
{
    private readonly Dictionary<string, BookingDto> _cache = new();

    public bool TryGet(string key, out BookingDto dto) => _cache.TryGetValue(key, out dto!);
    public void Remember(string key, BookingDto dto) => _cache[key] = dto;
}

class AvailabilityService
{
    private readonly HashSet<string> _blockedSlots = new();

    public bool IsSlotAvailable(Guid branchId, Guid staffId, DateTime start, DateTime end)
    {
        var token = Token(branchId, staffId, start, end);
        return !_blockedSlots.Contains(token);
    }

    public void BlockSlot(Guid branchId, Guid staffId, DateTime start, DateTime end)
    {
        var token = Token(branchId, staffId, start, end);
        _blockedSlots.Add(token);
    }

    private static string Token(Guid branchId, Guid staffId, DateTime start, DateTime end) => $"{branchId}:{staffId}:{start:o}:{end:o}";
}
