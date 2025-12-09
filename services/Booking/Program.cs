using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var bookings = new Dictionary<Guid, BookingResponse>();

app.MapGet("/health", () => Results.Ok(new { service = "booking", status = "ok" }));

app.MapPost("/bookings", ([FromBody] BookingRequest request, [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey) =>
{
    if (request.StartUtc >= request.EndUtc)
    {
        return Results.BadRequest(new { error = "invalid_time_range" });
    }

    var bookingId = Guid.NewGuid();
    var booking = new BookingResponse(
        bookingId,
        request.BranchId,
        request.ClientId,
        request.StaffId,
        request.StartUtc,
        request.EndUtc,
        "Confirmed",
        idempotencyKey ?? Guid.NewGuid().ToString("N")
    );

    bookings[bookingId] = booking;

    return Results.Created($"/bookings/{bookingId}", booking);
});

app.MapGet("/calendar", ([FromQuery] Guid branchId, [FromQuery] DateTime from, [FromQuery] DateTime to) =>
{
    var calendar = bookings.Values
        .Where(x => x.BranchId == branchId && x.StartUtc >= from && x.EndUtc <= to)
        .OrderBy(x => x.StartUtc);

    return Results.Ok(calendar);
});

app.MapPatch("/bookings/{id:guid}/status", ([FromRoute] Guid id, [FromBody] StatusChangeRequest request) =>
{
    if (!bookings.TryGetValue(id, out var booking))
    {
        return Results.NotFound();
    }

    var updated = booking with { Status = request.Status };
    bookings[id] = updated;

    return Results.Ok(updated);
});

app.Run();

record BookingRequest(Guid BranchId, Guid ClientId, Guid StaffId, DateTime StartUtc, DateTime EndUtc);
record BookingResponse(Guid Id, Guid BranchId, Guid ClientId, Guid StaffId, DateTime StartUtc, DateTime EndUtc, string Status, string IdempotencyKey);
record StatusChangeRequest(string Status);
