using MediatR;
using ReportsService.Application.Abstractions;
using ReportsService.Domain.Entities;

namespace ReportsService.Application.Reports.Queries.GetBranchDaily;

public sealed record GetBranchDailyQuery(Guid BranchId) : IRequest<IEnumerable<BranchDailyReport>>;

public sealed class GetBranchDailyQueryHandler : IRequestHandler<GetBranchDailyQuery, IEnumerable<BranchDailyReport>>
{
    private readonly IReportRepository _repository;

    public GetBranchDailyQueryHandler(IReportRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<BranchDailyReport>> Handle(GetBranchDailyQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_repository.BranchDaily(request.BranchId));
    }
}
