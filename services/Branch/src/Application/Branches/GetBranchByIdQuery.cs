using BranchService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace BranchService.Application.Branches;

public sealed record GetBranchByIdQuery(Guid Id) : IQuery<BranchDto?>;

public sealed class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, BranchDto?>
{
    private readonly IBranchRepository _repository;

    public GetBranchByIdQueryHandler(IBranchRepository repository)
    {
        _repository = repository;
    }

    public async Task<BranchDto?> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        var branch = await _repository.GetAsync(request.Id, cancellationToken);
        return branch is null
            ? null
            : new BranchDto(branch.Id, branch.Name, branch.Timezone, branch.Address);
    }
}
