using BookingService.Application.Appointments;
using MediatR;

namespace BookingService.Api;

public static class BookingEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/bookings");

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

        return app;
    }
}

public sealed record ScheduleAppointmentRequest(Guid BranchId, Guid ClientId, Guid StaffId, DateTime StartUtc, int DurationMinutes, string Source);
