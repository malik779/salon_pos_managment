using BookingService.Application.Abstractions;
using BookingService.Application.Bookings.Models;
using MediatR;

namespace BookingService.Application.Bookings.Queries.GetCalendar;

public sealed record GetCalendarQuery(Guid BranchId, DateTime FromUtc, DateTime ToUtc) : IRequest<IReadOnlyList<BookingDto>>;

public sealed class GetCalendarQueryHandler : IRequestHandler<GetCalendarQuery, IReadOnlyList<BookingDto>>
{
    private readonly IBookingRepository _repository;

    public GetCalendarQueryHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BookingDto>> Handle(GetCalendarQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _repository.ListAsync(request.BranchId, request.FromUtc, request.ToUtc, cancellationToken);
        return bookings
            .OrderBy(x => x.StartUtc)
            .Select(x => new BookingDto(x.Id, x.BranchId, x.ClientId, x.StaffId, x.StartUtc, x.EndUtc, x.Status, x.IdempotencyKey))
            .ToList();
    }
}
