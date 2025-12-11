using System.Collections.Generic;
using BookingService.Application.Abstractions;
using BookingService.Domain;
using FluentValidation;
using MediatR;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace BookingService.Application.Appointments;

public sealed record AppointmentDto(Guid Id, Guid BranchId, Guid ClientId, Guid StaffId, DateTime StartUtc, DateTime EndUtc, string Status, string Source);

public sealed record ScheduleAppointmentCommand(Guid BranchId, Guid ClientId, Guid StaffId, DateTime StartUtc, int DurationMinutes, string Source, string Actor)
    : ICommand<AppointmentDto>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "booking-service",
            Action: "AppointmentScheduled",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["branchId"] = BranchId,
                ["clientId"] = ClientId,
                ["staffId"] = StaffId,
                ["start"] = StartUtc,
                ["durationMinutes"] = DurationMinutes
            });
    }
}

public sealed class ScheduleAppointmentCommandValidator : AbstractValidator<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.Source).NotEmpty();
    }
}

public sealed class ScheduleAppointmentCommandHandler : IRequestHandler<ScheduleAppointmentCommand, AppointmentDto>
{
    private readonly IAppointmentRepository _repository;

    public ScheduleAppointmentCommandHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<AppointmentDto> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = new Appointment(
            Guid.NewGuid(),
            request.BranchId,
            request.ClientId,
            request.StaffId,
            request.StartUtc,
            TimeSpan.FromMinutes(request.DurationMinutes),
            request.Source);

        await _repository.AddAsync(appointment, cancellationToken);

        return new AppointmentDto(
            appointment.Id,
            appointment.BranchId,
            appointment.ClientId,
            appointment.StaffId,
            appointment.StartUtc,
            appointment.EndUtc,
            appointment.Status,
            appointment.Source);
    }
}
