using FluentValidation;
using MediatR;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("staff-service");

builder.Services.AddSingleton<StaffStore>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseServiceDefaults();

var staffGroup = app.MapGroup("/staff");

staffGroup.MapGet("", (StaffStore store) => Results.Ok(store.All()));
staffGroup.MapPost("", async (CreateStaffCommand command, IMediator mediator) => Results.Created("/staff", await mediator.Send(command)));
staffGroup.MapPost("/{id:guid}/commissions", async (Guid id, CalculateCommissionCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command with { StaffId = id });
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapPost("/attendance/checkin", async (CheckInCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));
app.MapPost("/attendance/checkout", async (CheckOutCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));

app.Run();

record StaffDto(Guid Id, string FullName, Guid DefaultBranchId, string Role);
record CommissionDto(Guid StaffId, decimal Amount, string RuleName);
record AttendanceDto(Guid StaffId, DateTime TimestampUtc, string Action);

record CreateStaffCommand(string FullName, Guid DefaultBranchId, string Role) : IRequest<StaffDto>;
record CalculateCommissionCommand(Guid StaffId, decimal SalesAmount, decimal CommissionRate) : IRequest<CommissionDto?>;
record CheckInCommand(Guid StaffId, DateTime TimestampUtc) : IRequest<AttendanceDto>;
record CheckOutCommand(Guid StaffId, DateTime TimestampUtc) : IRequest<AttendanceDto>;

class CreateStaffCommandValidator : AbstractValidator<CreateStaffCommand>
{
    public CreateStaffCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.DefaultBranchId).NotEmpty();
    }
}

class CalculateCommissionCommandValidator : AbstractValidator<CalculateCommissionCommand>
{
    public CalculateCommissionCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.SalesAmount).GreaterThan(0);
    }
}

class StaffStore
{
    private readonly Dictionary<Guid, StaffDto> _staff = new();

    public StaffDto Add(StaffDto dto)
    {
        _staff[dto.Id] = dto;
        return dto;
    }

    public StaffDto? Get(Guid id) => _staff.TryGetValue(id, out var dto) ? dto : null;
    public IEnumerable<StaffDto> All() => _staff.Values;
}

class CreateStaffHandler : IRequestHandler<CreateStaffCommand, StaffDto>
{
    private readonly StaffStore _store;

    public CreateStaffHandler(StaffStore store)
    {
        _store = store;
    }

    public Task<StaffDto> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = new StaffDto(Guid.NewGuid(), request.FullName, request.DefaultBranchId, request.Role);
        _store.Add(staff);
        return Task.FromResult(staff);
    }
}

class CalculateCommissionHandler : IRequestHandler<CalculateCommissionCommand, CommissionDto?>
{
    private readonly StaffStore _store;

    public CalculateCommissionHandler(StaffStore store)
    {
        _store = store;
    }

    public Task<CommissionDto?> Handle(CalculateCommissionCommand request, CancellationToken cancellationToken)
    {
        var staff = _store.Get(request.StaffId);
        if (staff is null)
        {
            return Task.FromResult<CommissionDto?>(null);
        }

        var amount = request.SalesAmount * request.CommissionRate;
        return Task.FromResult<CommissionDto?>(new CommissionDto(staff.Id, amount, "percentage"));
    }
}

class CheckInHandler : IRequestHandler<CheckInCommand, AttendanceDto>
{
    public Task<AttendanceDto> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new AttendanceDto(request.StaffId, request.TimestampUtc, "checkin"));
    }
}

class CheckOutHandler : IRequestHandler<CheckOutCommand, AttendanceDto>
{
    public Task<AttendanceDto> Handle(CheckOutCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new AttendanceDto(request.StaffId, request.TimestampUtc, "checkout"));
    }
}
