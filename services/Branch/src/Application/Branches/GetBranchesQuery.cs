using BranchService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace BranchService.Application.Branches;

public sealed record GetBranchesQuery(
    int PageNumber,
    int PageSize,
    string? SearchTerm) : IQuery<PaginatedResponse<BranchDto>>;

public sealed class GetBranchesQueryHandler : IRequestHandler<GetBranchesQuery, PaginatedResponse<BranchDto>>
{
    private readonly IBranchRepository _repository;

    public GetBranchesQueryHandler(IBranchRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResponse<BranchDto>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetBranchesAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            cancellationToken);

        var branchDtos = items.Select(b => new BranchDto(b.Id, b.Name, b.Timezone, b.Address)).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PaginatedResponse<BranchDto>(
            branchDtos,
            request.PageNumber,
            request.PageSize,
            totalCount,
            totalPages);
    }
}

public record PaginatedResponse<T>(
    List<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages);
