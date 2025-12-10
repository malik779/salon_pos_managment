using BookingService.Application.Abstractions;
using BookingService.Application.Bookings.Models;
using MediatR;

namespace BookingService.Application.Bookings.Queries.GetBooking;

public sealed record GetBookingQuery(Guid BookingId) : IRequest<BookingDto?>;

public sealed class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, BookingDto?>
{
    private readonly IBookingRepository _repository;

    public GetBookingQueryHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookingDto?> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetAsync(request.BookingId, cancellationToken);
        return booking is null ? null : new BookingDto(booking.Id, booking.BranchId, booking.ClientId, booking.StaffId, booking.StartUtc, booking.EndUtc, booking.Status, booking.IdempotencyKey);
    }
}
