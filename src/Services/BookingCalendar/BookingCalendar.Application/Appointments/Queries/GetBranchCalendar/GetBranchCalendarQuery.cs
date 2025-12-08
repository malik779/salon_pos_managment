using BookingCalendar.Application.Abstractions;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendar.Application.Appointments.Queries.GetBranchCalendar;

public sealed record AppointmentSlot(Guid Id, DateTime StartUtc, DateTime EndUtc, string Status);

public sealed record GetBranchCalendarQuery(Guid BranchId, DateTime RangeStart, DateTime RangeEnd) : IQuery<IReadOnlyCollection<AppointmentSlot>>;

public sealed class GetBranchCalendarQueryValidator : AbstractValidator<GetBranchCalendarQuery>
{
    public GetBranchCalendarQueryValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.RangeEnd).GreaterThan(x => x.RangeStart);
    }
}

public sealed class GetBranchCalendarQueryHandler(IBookingDbContext context)
    : IQueryHandler<GetBranchCalendarQuery, IReadOnlyCollection<AppointmentSlot>>
{
    public async Task<Result<IReadOnlyCollection<AppointmentSlot>>> Handle(GetBranchCalendarQuery request, CancellationToken cancellationToken)
    {
        var slots = await context.Appointments
            .AsNoTracking()
            .Where(a =>
                a.BranchId == request.BranchId &&
                a.StartUtc >= request.RangeStart &&
                a.EndUtc <= request.RangeEnd)
            .OrderBy(a => a.StartUtc)
            .Select(a => new AppointmentSlot(a.Id, a.StartUtc, a.EndUtc, a.Status))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<AppointmentSlot>>(slots);
    }
}
