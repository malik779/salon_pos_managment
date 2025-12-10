using FluentValidation;
using MediatR;
using StaffService.Application.Abstractions;
using StaffService.Application.Staff.Models;
using StaffService.Domain.Entities;

namespace StaffService.Application.Staff.Commands.CreateStaff;

public sealed record CreateStaffCommand(string FullName, Guid DefaultBranchId, string Role) : IRequest<StaffDto>;

public sealed class CreateStaffCommandValidator : AbstractValidator<CreateStaffCommand>
{
    public CreateStaffCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.DefaultBranchId).NotEmpty();
    }
}

public sealed class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, StaffDto>
{
    private readonly IStaffRepository _repository;

    public CreateStaffCommandHandler(IStaffRepository repository)
    {
        _repository = repository;
    }

    public async Task<StaffDto> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = new StaffMember(Guid.NewGuid(), request.FullName, request.DefaultBranchId, request.Role);
        await _repository.AddAsync(staff, cancellationToken);
        return new StaffDto(staff.Id, staff.FullName, staff.DefaultBranchId, staff.Role);
    }
}
