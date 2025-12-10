using FluentValidation;
using MediatR;
using StaffService.Application.Staff.Models;

namespace StaffService.Application.Staff.Commands.Attendance;

public sealed record CheckInCommand(Guid StaffId, DateTime TimestampUtc) : IRequest<AttendanceDto>;
public sealed record CheckOutCommand(Guid StaffId, DateTime TimestampUtc) : IRequest<AttendanceDto>;

public sealed class CheckInCommandValidator : AbstractValidator<CheckInCommand>
{
    public CheckInCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty();
    }
}

public sealed class CheckOutCommandValidator : AbstractValidator<CheckOutCommand>
{
    public CheckOutCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty();
    }
}

public sealed class CheckInCommandHandler : IRequestHandler<CheckInCommand, AttendanceDto>
{
    public Task<AttendanceDto> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new AttendanceDto(request.StaffId, request.TimestampUtc, "checkin"));
    }
}

public sealed class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, AttendanceDto>
{
    public Task<AttendanceDto> Handle(CheckOutCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new AttendanceDto(request.StaffId, request.TimestampUtc, "checkout"));
    }
}
