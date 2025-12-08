using BuildingBlocks.Domain.Abstractions;

namespace StaffPayroll.Domain.Staff;

public sealed class StaffMember : AuditableEntity, IAggregateRoot
{
    private StaffMember()
    {
    }

    private StaffMember(Guid id, Guid userId, decimal commissionRate, string employmentType)
    {
        Id = id;
        UserId = userId;
        CommissionRate = commissionRate;
        EmploymentType = employmentType;
        IsActive = true;
    }

    public Guid UserId { get; private set; } = Guid.Empty;
    public decimal CommissionRate { get; private set; }
        = 0;
    public string EmploymentType { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    public static StaffMember Register(Guid userId, decimal commissionRate, string employmentType)
        => new(Guid.NewGuid(), userId, commissionRate, employmentType);
}
