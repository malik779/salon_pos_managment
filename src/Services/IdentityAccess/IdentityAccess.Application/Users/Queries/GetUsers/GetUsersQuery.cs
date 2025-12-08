using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Models;
using BuildingBlocks.Domain.Primitives;
using IdentityAccess.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using SalonSuite.Contracts.Identity;

namespace IdentityAccess.Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery(int PageNumber = 1, int PageSize = 50) : IQuery<PagedResult<UserSummary>>;

public sealed class GetUsersQueryHandler(IIdentityAccessDbContext context)
    : IQueryHandler<GetUsersQuery, PagedResult<UserSummary>>
{
    public async Task<Result<PagedResult<UserSummary>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = context.Users
            .AsNoTracking()
            .OrderBy(u => u.Email);

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserSummary(u.Id, u.Email, u.Name, u.DefaultBranchId, u.IsActive))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<UserSummary>(items, request.PageNumber, request.PageSize, total);
        return Result.Success(result);
    }
}
