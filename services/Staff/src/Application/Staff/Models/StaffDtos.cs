namespace StaffService.Application.Staff.Models;

public sealed record StaffDto(Guid Id, string FullName, Guid DefaultBranchId, string Role);
public sealed record CommissionDto(Guid StaffId, decimal Amount, string RuleName);
public sealed record AttendanceDto(Guid StaffId, DateTime TimestampUtc, string Action);
