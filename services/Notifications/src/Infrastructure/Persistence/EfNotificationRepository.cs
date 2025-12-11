using Microsoft.EntityFrameworkCore;
using NotificationsService.Application.Abstractions;
using NotificationsService.Domain.Entities;

namespace NotificationsService.Infrastructure.Persistence;

public sealed class EfNotificationRepository : INotificationRepository
{
    private readonly NotificationsDbContext _dbContext;

    public EfNotificationRepository(NotificationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddTemplateAsync(NotificationTemplate template, CancellationToken cancellationToken)
    {
        await _dbContext.Templates.AddAsync(template, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> ListTemplatesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Templates.AsNoTracking().ToListAsync(cancellationToken);
    }
}
