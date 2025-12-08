using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SyncOffline.Application.Abstractions;
using SyncOffline.Domain.Devices;

namespace SyncOffline.Application.Devices.Commands.RegisterDevice;

public sealed record RegisterDeviceCommand(string DeviceId, Guid BranchId, string Platform) : ICommand<Guid>;

public sealed class RegisterDeviceCommandValidator : AbstractValidator<RegisterDeviceCommand>
{
    public RegisterDeviceCommandValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Platform).NotEmpty().MaximumLength(50);
    }
}

public sealed class RegisterDeviceCommandHandler(ISyncDbContext context)
    : ICommandHandler<RegisterDeviceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.Devices.AnyAsync(d => d.DeviceId == request.DeviceId, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(new Error("SyncOffline.DeviceExists", "Device already registered"));
        }

        var device = DeviceRegistration.Register(request.DeviceId, request.BranchId, request.Platform);
        context.Devices.Add(device);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(device.Id);
    }
}
