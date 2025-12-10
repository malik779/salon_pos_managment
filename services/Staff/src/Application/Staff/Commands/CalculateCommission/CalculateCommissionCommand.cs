using FluentValidation;
using MediatR;
using StaffService.Application.Staff.Models;

namespace StaffService.Application.Staff.Commands.CalculateCommission;

public sealed record CalculateCommissionCommand(Guid StaffId, decimal SalesAmount, decimal CommissionRate) : IRequest<CommissionDto?>;

public sealed class CalculateCommissionCommandValidator : AbstractValidator<CalculateCommissionCommand>
{
    public CalculateCommissionCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.SalesAmount).GreaterThan(0);
    }
}

public sealed class CalculateCommissionCommandHandler : IRequestHandler<CalculateCommissionCommand, CommissionDto?>
{
    public Task<CommissionDto?> Handle(CalculateCommissionCommand request, CancellationToken cancellationToken)
    {
        var amount = request.SalesAmount * request.CommissionRate;
        return Task.FromResult<CommissionDto?>(new CommissionDto(request.StaffId, amount, "percentage"));
    }
}
