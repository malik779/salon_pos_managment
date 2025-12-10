using MediatR;

namespace ReportsService.Application.Reports.Commands.RebuildReports;

public sealed record RebuildReportsCommand(Guid BranchId) : IRequest<string>;

public sealed class RebuildReportsCommandHandler : IRequestHandler<RebuildReportsCommand, string>
{
    public Task<string> Handle(RebuildReportsCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"rebuild_started_for_{request.BranchId}");
    }
}
