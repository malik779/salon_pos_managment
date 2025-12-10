using BranchService.Application.Abstractions;
using BranchService.Application.Branches.Models;
using MediatR;

namespace BranchService.Application.Branches.Queries.GetBranch;

public sealed record GetBranchQuery(Guid Id) : IRequest<BranchDto?>;

public sealed class GetBranchQueryHandler : IRequestHandler<GetBranchQuery, BranchDto?>
{
    private readonly IBranchRepository _repository;

    public GetBranchQueryHandler(IBranchRepository repository)
    {
        _repository = repository;
    }

    public async Task<BranchDto?> Handle(GetBranchQuery request, CancellationToken cancellationToken)
    {
        var branch = await _repository.GetAsync(request.Id, cancellationToken);
        return branch is null ? null : new BranchDto(branch.Id, branch.Name, branch.Timezone, branch.Address);
    }
}
