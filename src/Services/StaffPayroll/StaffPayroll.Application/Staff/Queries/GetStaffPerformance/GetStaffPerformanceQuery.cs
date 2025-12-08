using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using StaffPayroll.Application.Abstractions;
using StaffPayroll.Domain.Staff;

namespace StaffPayroll.Application.Staff.Queries.GetStaffPerformance;

public sealed record StaffPerformanceDto(Guid StaffId, decimal TotalSales, decimal CommissionTotal, int CompletedAppointments);

public sealed record GetStaffPerformanceQuery(Guid StaffId, DateTime StartDate, DateTime EndDate) : IQuery<StaffPerformanceDto>;

public sealed class GetStaffPerformanceQueryHandler(IStaffDbContext context)
    : IQueryHandler<GetStaffPerformanceQuery, StaffPerformanceDto>
{
    public async Task<Result<StaffPerformanceDto>> Handle(GetStaffPerformanceQuery request, CancellationToken cancellationToken)
    {
        var staff = await context.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.StaffId, cancellationToken);

        if (staff is null)
        {
            return Result.Failure<StaffPerformanceDto>(new Error("StaffPayroll.StaffNotFound", "Staff member not found"));
        }

        // Placeholder aggregations until reporting service is wired up
        var totalSales = staff.CommissionRate * 10000m;
        var commission = totalSales * staff.CommissionRate;
        var completedAppointments = (int)Math.Round(totalSales / 150m);

        var dto = new StaffPerformanceDto(staff.Id, totalSales, commission, completedAppointments);
        return Result.Success(dto);
    }
}
