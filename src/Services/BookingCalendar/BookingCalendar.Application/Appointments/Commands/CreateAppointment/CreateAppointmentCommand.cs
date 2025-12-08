using BookingCalendar.Application.Abstractions;
using BookingCalendar.Domain.Appointments;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendar.Application.Appointments.Commands.CreateAppointment;

public sealed record CreateAppointmentCommand(
    Guid BranchId,
    Guid ClientId,
    Guid StaffId,
    IReadOnlyCollection<Guid> ServiceIds,
    DateTime StartUtc,
    DateTime EndUtc) : ICommand<Guid>;

public sealed class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.ServiceIds).NotEmpty();
        RuleFor(x => x.EndUtc).GreaterThan(x => x.StartUtc);
    }
}

public sealed class CreateAppointmentCommandHandler(IBookingDbContext context)
    : ICommandHandler<CreateAppointmentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var overlap = await context.Appointments.AnyAsync(a =>
            a.BranchId == request.BranchId &&
            a.StaffId == request.StaffId &&
            a.StartUtc < request.EndUtc &&
            request.StartUtc < a.EndUtc,
            cancellationToken);

        if (overlap)
        {
            return Result.Failure<Guid>(new Error("BookingCalendar.Conflict", "Staff member already has an appointment in this slot"));
        }

        var appointment = Appointment.Schedule(request.BranchId, request.ClientId, request.StaffId, request.ServiceIds, request.StartUtc, request.EndUtc);
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(appointment.Id);
    }
}
