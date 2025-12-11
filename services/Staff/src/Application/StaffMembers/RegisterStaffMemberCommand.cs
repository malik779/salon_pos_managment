using System.Collections.Generic;
using FluentValidation;
using MediatR;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;
using StaffService.Application.Abstractions;
using StaffService.Domain;

namespace StaffService.Application.StaffMembers;

public sealed record StaffMemberDto(Guid Id, Guid UserId, Guid DefaultBranchId, string Role, bool IsActive);

public sealed record RegisterStaffMemberCommand(Guid UserId, Guid DefaultBranchId, string Role, string Actor)
    : ICommand<StaffMemberDto>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "staff-service",
            Action: "StaffMemberRegistered",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["userId"] = UserId,
                ["branchId"] = DefaultBranchId,
                ["role"] = Role
            });
    }
}

public sealed class RegisterStaffMemberCommandValidator : AbstractValidator<RegisterStaffMemberCommand>
{
    public RegisterStaffMemberCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DefaultBranchId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty();
    }
}

public sealed class RegisterStaffMemberCommandHandler : IRequestHandler<RegisterStaffMemberCommand, StaffMemberDto>
{
    private readonly IStaffRepository _repository;

    public RegisterStaffMemberCommandHandler(IStaffRepository repository)
    {
        _repository = repository;
    }

    public async Task<StaffMemberDto> Handle(RegisterStaffMemberCommand request, CancellationToken cancellationToken)
    {
        var staff = new StaffMember(Guid.NewGuid(), request.UserId, request.DefaultBranchId, request.Role);
        await _repository.AddAsync(staff, cancellationToken);
        return new StaffMemberDto(staff.Id, staff.UserId, staff.DefaultBranchId, staff.Role, staff.IsActive);
    }
}
