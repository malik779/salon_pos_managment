using BranchConfiguration.Application.Abstractions;
using BranchConfiguration.Domain;
using BranchConfiguration.Domain.Branches;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace BranchConfiguration.Application.Branches.Queries.GetBranchById;

public sealed record BranchDto(
    Guid Id,
    string Name,
    string Timezone,
    string Address,
    decimal DefaultTaxRate,
    TimeSpan OpenTimeUtc,
    TimeSpan CloseTimeUtc);

public sealed record GetBranchByIdQuery(Guid BranchId) : IQuery<BranchDto>;

public sealed class GetBranchByIdQueryHandler(IBranchConfigurationDbContext context)
    : IQueryHandler<GetBranchByIdQuery, BranchDto>
{
    public async Task<Result<BranchDto>> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        var branch = await context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BranchId, cancellationToken);

        if (branch is null)
        {
            return Result.Failure<BranchDto>(BranchErrors.NotFound);
        }

        var dto = new BranchDto(branch.Id, branch.Name, branch.Timezone, branch.Address, branch.DefaultTaxRate, branch.OpenTimeUtc, branch.CloseTimeUtc);
        return Result.Success(dto);
    }
}
