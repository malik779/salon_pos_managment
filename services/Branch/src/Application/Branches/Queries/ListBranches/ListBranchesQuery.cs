using BranchService.Application.Abstractions;
using BranchService.Application.Branches.Models;
using MediatR;

namespace BranchService.Application.Branches.Queries.ListBranches;

public sealed record ListBranchesQuery() : IRequest<IReadOnlyList<BranchDto>>;

public sealed class ListBranchesQueryHandler : IRequestHandler<ListBranchesQuery, IReadOnlyList<BranchDto>>
{
    private readonly IBranchRepository _repository;

    public ListBranchesQueryHandler(IBranchRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BranchDto>> Handle(ListBranchesQuery request, CancellationToken cancellationToken)
    {
        var branches = await _repository.ListAsync(cancellationToken);
        return branches.Select(b => new BranchDto(b.Id, b.Name, b.Timezone, b.Address)).ToList();
    }
}
