using MediatR;
using ReportsService.Application.Abstractions;
using ReportsService.Domain.Entities;

namespace ReportsService.Application.Reports.Queries.GetKpis;

public sealed record GetKpisQuery() : IRequest<IEnumerable<ReportKpi>>;

public sealed class GetKpisQueryHandler : IRequestHandler<GetKpisQuery, IEnumerable<ReportKpi>>
{
    private readonly IReportRepository _repository;

    public GetKpisQueryHandler(IReportRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<ReportKpi>> Handle(GetKpisQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_repository.LatestKpis());
    }
}
