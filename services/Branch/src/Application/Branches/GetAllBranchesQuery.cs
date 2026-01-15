using BranchService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace BranchService.Application.Branches;

/// <summary>
/// Query to fetch all branches without pagination
/// Optimized for dropdowns, selects, and other UI components that need complete lists
/// Performance: Uses AsNoTracking for read-only queries to reduce memory overhead
/// </summary>
public sealed record GetAllBranchesQuery(string? SearchTerm = null) : IQuery<List<BranchDto>>;

public sealed class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, List<BranchDto>>
{
    private readonly IBranchRepository _repository;

    public GetAllBranchesQueryHandler(IBranchRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
    {
        var branches = await _repository.GetAllAsync(request.SearchTerm, cancellationToken);
        return branches.Select(b => new BranchDto(b.Id, b.Name, b.Timezone, b.Address)).ToList();
    }
}
