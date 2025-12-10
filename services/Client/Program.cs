using FluentValidation;
using MediatR;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("client-service");

builder.Services.AddSingleton<ClientStore>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseServiceDefaults();

var clients = app.MapGroup("/clients");

clients.MapGet("", (ClientStore store) => Results.Ok(store.All()));
clients.MapGet("/{id:guid}", (Guid id, ClientStore store) =>
{
    var client = store.Get(id);
    return client is null ? Results.NotFound() : Results.Ok(client);
});

clients.MapPost("", async (CreateClientCommand command, IMediator mediator) => Results.Created("/clients", await mediator.Send(command)));
clients.MapPost("/{id:guid}/consents", async (Guid id, CaptureConsentCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command with { ClientId = id })));
clients.MapGet("/{id:guid}/packages", (Guid id, ClientStore store) => Results.Ok(store.GetPackages(id)));

app.Run();

record ClientDto(Guid Id, string FirstName, string LastName, string Phone, string Email);
record ConsentDto(Guid ClientId, string Version, DateTime CapturedAtUtc);
record PackageBalanceDto(Guid ClientId, string PackageName, int RemainingSessions);

record CreateClientCommand(string FirstName, string LastName, string Phone, string Email) : IRequest<ClientDto>;
record CaptureConsentCommand(Guid ClientId, string Version) : IRequest<ConsentDto>;
record AddPackageCommand(Guid ClientId, string PackageName, int Sessions) : IRequest<PackageBalanceDto>;

class CreateClientValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Phone).NotEmpty();
    }
}

class CaptureConsentValidator : AbstractValidator<CaptureConsentCommand>
{
    public CaptureConsentValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
    }
}

class ClientStore
{
    private readonly Dictionary<Guid, ClientDto> _clients = new();
    private readonly Dictionary<Guid, List<ConsentDto>> _consents = new();
    private readonly Dictionary<Guid, List<PackageBalanceDto>> _packages = new();

    public IEnumerable<ClientDto> All() => _clients.Values;
    public ClientDto? Get(Guid id) => _clients.TryGetValue(id, out var dto) ? dto : null;
    public IEnumerable<PackageBalanceDto> GetPackages(Guid id) => _packages.TryGetValue(id, out var list) ? list : Enumerable.Empty<PackageBalanceDto>();

    public void Add(ClientDto dto) => _clients[dto.Id] = dto;
    public void AddConsent(ConsentDto dto)
    {
        if (!_consents.TryGetValue(dto.ClientId, out var list))
        {
            list = new List<ConsentDto>();
            _consents[dto.ClientId] = list;
        }
        list.Add(dto);
    }

    public void AddPackage(PackageBalanceDto dto)
    {
        if (!_packages.TryGetValue(dto.ClientId, out var list))
        {
            list = new List<PackageBalanceDto>();
            _packages[dto.ClientId] = list;
        }
        list.Add(dto);
    }
}

class CreateClientHandler : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly ClientStore _store;

    public CreateClientHandler(ClientStore store)
    {
        _store = store;
    }

    public Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var client = new ClientDto(Guid.NewGuid(), request.FirstName, request.LastName, request.Phone, request.Email);
        _store.Add(client);
        return Task.FromResult(client);
    }
}

class CaptureConsentHandler : IRequestHandler<CaptureConsentCommand, ConsentDto>
{
    private readonly ClientStore _store;

    public CaptureConsentHandler(ClientStore store)
    {
        _store = store;
    }

    public Task<ConsentDto> Handle(CaptureConsentCommand request, CancellationToken cancellationToken)
    {
        var consent = new ConsentDto(request.ClientId, request.Version, DateTime.UtcNow);
        _store.AddConsent(consent);
        return Task.FromResult(consent);
    }
}

class AddPackageHandler : IRequestHandler<AddPackageCommand, PackageBalanceDto>
{
    private readonly ClientStore _store;

    public AddPackageHandler(ClientStore store)
    {
        _store = store;
    }

    public Task<PackageBalanceDto> Handle(AddPackageCommand request, CancellationToken cancellationToken)
    {
        var package = new PackageBalanceDto(request.ClientId, request.PackageName, request.Sessions);
        _store.AddPackage(package);
        return Task.FromResult(package);
    }
}
