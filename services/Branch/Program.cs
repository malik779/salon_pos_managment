using FluentValidation;
using MediatR;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("branch-service");

builder.Services.AddSingleton<BranchStore>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseServiceDefaults();

var branches = app.MapGroup("/branches");

branches.MapGet("", (BranchStore store) => Results.Ok(store.All()));
branches.MapGet("/{id:guid}", (Guid id, BranchStore store) =>
{
    var branch = store.Get(id);
    return branch is null ? Results.NotFound() : Results.Ok(branch);
});

branches.MapPost("", async (CreateBranchCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.Created($"/branches/{result.Id}", result);
});

branches.MapPut("/{id:guid}/settings", async (Guid id, UpdateBranchSettingsCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command with { BranchId = id });
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.Run();

record BranchDto(Guid Id, string Name, string Timezone, string Address);
record BranchSettingsDto(Guid BranchId, string ReceiptTemplate, TimeSpan BookingWindowStart, TimeSpan BookingWindowEnd);

record CreateBranchCommand(string Name, string Timezone, string Address) : IRequest<BranchDto>;
record UpdateBranchSettingsCommand(Guid BranchId, string ReceiptTemplate, TimeSpan BookingWindowStart, TimeSpan BookingWindowEnd) : IRequest<BranchSettingsDto?>;

class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Timezone).NotEmpty();
    }
}

class UpdateBranchSettingsCommandValidator : AbstractValidator<UpdateBranchSettingsCommand>
{
    public UpdateBranchSettingsCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.ReceiptTemplate).NotEmpty();
    }
}

class CreateBranchHandler : IRequestHandler<CreateBranchCommand, BranchDto>
{
    private readonly BranchStore _store;

    public CreateBranchHandler(BranchStore store)
    {
        _store = store;
    }

    public Task<BranchDto> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = new BranchDto(Guid.NewGuid(), request.Name, request.Timezone, request.Address);
        _store.Upsert(branch);
        return Task.FromResult(branch);
    }
}

class UpdateBranchSettingsHandler : IRequestHandler<UpdateBranchSettingsCommand, BranchSettingsDto?>
{
    private readonly BranchStore _store;

    public UpdateBranchSettingsHandler(BranchStore store)
    {
        _store = store;
    }

    public Task<BranchSettingsDto?> Handle(UpdateBranchSettingsCommand request, CancellationToken cancellationToken)
    {
        var branch = _store.Get(request.BranchId);
        if (branch is null)
        {
            return Task.FromResult<BranchSettingsDto?>(null);
        }

        var settings = new BranchSettingsDto(branch.Id, request.ReceiptTemplate, request.BookingWindowStart, request.BookingWindowEnd);
        _store.UpsertSettings(settings);
        return Task.FromResult<BranchSettingsDto?>(settings);
    }
}

class BranchStore
{
    private readonly Dictionary<Guid, BranchDto> _branches = new();
    private readonly Dictionary<Guid, BranchSettingsDto> _settings = new();

    public IEnumerable<BranchDto> All() => _branches.Values;
    public BranchDto? Get(Guid id) => _branches.TryGetValue(id, out var dto) ? dto : null;

    public void Upsert(BranchDto dto) => _branches[dto.Id] = dto;
    public void UpsertSettings(BranchSettingsDto dto) => _settings[dto.BranchId] = dto;
}
