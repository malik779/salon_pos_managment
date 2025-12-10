using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using StaffService.Application.Staff.Commands.Attendance;
using StaffService.Application.Staff.Commands.CalculateCommission;
using StaffService.Application.Staff.Commands.CreateStaff;
using StaffService.Application.Staff.Queries.ListStaff;

namespace StaffService.Api;

public static class StaffEndpoints
{
    public static IEndpointRouteBuilder MapStaffEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/staff");

        group.MapGet("", async (IMediator mediator) => Results.Ok(await mediator.Send(new ListStaffQuery())));

        group.MapPost("", async (CreateStaffCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/staff/{result.Id}", result);
        });

        group.MapPost("/{id:guid}/commissions", async (Guid id, CalculateCommissionCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { StaffId = id });
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        app.MapPost("/attendance/checkin", async (CheckInCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));
        app.MapPost("/attendance/checkout", async (CheckOutCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));

        return app;
    }
}
