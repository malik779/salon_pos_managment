using BookingService.Application.Abstractions;
using BookingService.Application.Bookings.Models;
using BookingService.Domain.Entities;
using FluentValidation;
using MediatR;
using Salon.ServiceDefaults.Messaging;

namespace BookingService.Application.Bookings.Commands.CreateBooking;

public sealed record CreateBookingCommand(Guid BranchId, Guid ClientId, Guid StaffId, DateTime StartUtc, DateTime EndUtc, string IdempotencyKey) : IRequest<BookingDto>;

public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.EndUtc).GreaterThan(x => x.StartUtc);
        RuleFor(x => x.IdempotencyKey).NotEmpty();
    }
}

public sealed class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingRepository _repository;
    private readonly IIdempotencyStore _idempotencyStore;
    private readonly IAvailabilityService _availabilityService;
    private readonly IEventPublisher _eventPublisher;

    public CreateBookingCommandHandler(IBookingRepository repository, IIdempotencyStore idempotencyStore, IAvailabilityService availabilityService, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _idempotencyStore = idempotencyStore;
        _availabilityService = availabilityService;
        _eventPublisher = eventPublisher;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var cached = await _idempotencyStore.GetAsync(request.IdempotencyKey, cancellationToken);
        if (cached is not null)
        {
            return ToDto(cached);
        }

        if (!_availabilityService.IsAvailable(request.BranchId, request.StaffId, request.StartUtc, request.EndUtc))
        {
            throw new InvalidOperationException("slot_unavailable");
        }

        var booking = new Booking(Guid.NewGuid(), request.BranchId, request.ClientId, request.StaffId, request.StartUtc, request.EndUtc, "Confirmed", request.IdempotencyKey);
        await _repository.AddAsync(booking, cancellationToken);
        await _idempotencyStore.RememberAsync(request.IdempotencyKey, booking, cancellationToken);
        _availabilityService.Block(request.BranchId, request.StaffId, request.StartUtc, request.EndUtc);

        await _eventPublisher.PublishAsync("booking.appointments", "booking.created", new
        {
            booking.Id,
            booking.BranchId,
            booking.StaffId,
            booking.StartUtc,
            booking.EndUtc
        }, cancellationToken);

        return ToDto(booking);
    }

    private static BookingDto ToDto(Booking booking) => new(booking.Id, booking.BranchId, booking.ClientId, booking.StaffId, booking.StartUtc, booking.EndUtc, booking.Status, booking.IdempotencyKey);
}
