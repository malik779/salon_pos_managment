using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StaffPayroll.Application.Abstractions;
using StaffPayroll.Domain.Staff;

namespace StaffPayroll.Application.Staff.Commands.RegisterStaff;

public sealed record RegisterStaffCommand(Guid UserId, decimal CommissionRate, string EmploymentType) : ICommand<Guid>;

public sealed class RegisterStaffCommandValidator : AbstractValidator<RegisterStaffCommand>
{
    public RegisterStaffCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CommissionRate).InclusiveBetween(0, 1);
        RuleFor(x => x.EmploymentType).NotEmpty().MaximumLength(50);
    }
}

public sealed class RegisterStaffCommandHandler(IStaffDbContext context)
    : ICommandHandler<RegisterStaffCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterStaffCommand request, CancellationToken cancellationToken)
    {
        var alreadyRegistered = await context.StaffMembers.AnyAsync(s => s.UserId == request.UserId, cancellationToken);
        if (alreadyRegistered)
        {
            return Result.Failure<Guid>(new Error("StaffPayroll.Exists", "Staff member already registered"));
        }

        var staff = StaffMember.Register(request.UserId, request.CommissionRate, request.EmploymentType);
        context.StaffMembers.Add(staff);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(staff.Id);
    }
}
