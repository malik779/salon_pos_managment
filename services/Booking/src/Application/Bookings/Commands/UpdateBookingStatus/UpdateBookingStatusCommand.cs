using BookingService.Application.Abstractions;
using BookingService.Application.Bookings.Models;
using FluentValidation;
using MediatR;

namespace BookingService.Application.Bookings.Commands.UpdateBookingStatus;

public sealed record UpdateBookingStatusCommand(Guid BookingId, string Status) : IRequest<BookingDto?>;

public sealed class UpdateBookingStatusCommandValidator : AbstractValidator<UpdateBookingStatusCommand>
{
    public UpdateBookingStatusCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
    }
}

public sealed class UpdateBookingStatusCommandHandler : IRequestHandler<UpdateBookingStatusCommand, BookingDto?>
{
    private readonly IBookingRepository _repository;

    public UpdateBookingStatusCommandHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookingDto?> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetAsync(request.BookingId, cancellationToken);
        if (booking is null)
        {
            return null;
        }

        booking.UpdateStatus(request.Status);
        return new BookingDto(booking.Id, booking.BranchId, booking.ClientId, booking.StaffId, booking.StartUtc, booking.EndUtc, booking.Status, booking.IdempotencyKey);
    }
}
