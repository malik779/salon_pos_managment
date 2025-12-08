using BookingCalendar.Domain.Appointments;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendar.Application.Abstractions;

public interface IBookingDbContext
{
    DbSet<Appointment> Appointments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
