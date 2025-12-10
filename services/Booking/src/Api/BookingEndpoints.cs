using BookingService.Application.Bookings.Commands.CreateBooking;
using BookingService.Application.Bookings.Commands.UpdateBookingStatus;
using BookingService.Application.Bookings.Queries.GetBooking;
using BookingService.Application.Bookings.Queries.GetCalendar;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BookingService.Api;

public static class BookingEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var bookings = app.MapGroup("/bookings");

        bookings.MapPost("", async (CreateBookingCommand command, HttpRequest request, IMediator mediator) =>
        {
            if (!request.Headers.TryGetValue("Idempotency-Key", out var key) || string.IsNullOrWhiteSpace(key))
            {
                return Results.BadRequest(new { error = "missing_idempotency_key" });
            }

            var result = await mediator.Send(command with { IdempotencyKey = key! }, request.HttpContext.RequestAborted);
            return Results.Created($"/bookings/{result.Id}", result);
        });

        bookings.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var booking = await mediator.Send(new GetBookingQuery(id));
            return booking is null ? Results.NotFound() : Results.Ok(booking);
        });

        bookings.MapPatch("/{id:guid}/status", async (Guid id, UpdateBookingStatusCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { BookingId = id });
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        app.MapGet("/calendar", async (Guid branchId, DateTime from, DateTime to, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCalendarQuery(branchId, from, to));
            return Results.Ok(result);
        });

        return app;
    }
}
