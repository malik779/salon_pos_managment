using BookingService.Application.Appointments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Api;

public static class BookingEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/booking");

        group.MapPost("", async (ScheduleAppointmentRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new ScheduleAppointmentCommand(request.BranchId, request.ClientId, request.StaffId, request.StartUtc, request.DurationMinutes, request.Source, actor);
            var result = await sender.Send(command);
            return Results.Created($"/bookings/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetAppointmentByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapGet("/all", async (ISender sender, [FromQuery] string? searchTerm = null) =>
        {
            var query = new GetAllAppointmentsQuery(searchTerm);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetAllAppointments");

        group.MapGet("/calendar", async ([FromQuery] Guid branchId, [FromQuery] DateTime from, [FromQuery] DateTime to, ISender sender) =>
        {
            var query = new GetCalendarQuery(branchId, from, to);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetBookingCalendar");

        return app;
    }
}

public sealed record ScheduleAppointmentRequest(Guid BranchId, Guid ClientId, Guid StaffId, DateTime StartUtc, int DurationMinutes, string Source);
