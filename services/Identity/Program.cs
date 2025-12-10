using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("identity-service");

builder.Services.AddSingleton<UserStore>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();
app.UseServiceDefaults();

app.MapPost("/auth/token", ([FromBody] TokenRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new { error = "invalid_credentials" });
    }

    var token = new TokenResponse(
        AccessToken: Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
        RefreshToken: Guid.NewGuid().ToString("N"),
        ExpiresIn: 3600
    );

    return Results.Ok(token);
});

app.MapPost("/users", async ([FromBody] CreateUserCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.Created($"/users/{result.Id}", result);
});

app.MapGet("/users", (UserStore store) => Results.Ok(store.Users));
app.MapGet("/users/{id:guid}", (Guid id, UserStore store) =>
{
    var user = store.Users.FirstOrDefault(x => x.Id == id);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapPost("/devices/register", ([FromBody] DeviceRegistrationRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.DevicePublicKey))
    {
        return Results.BadRequest(new { error = "invalid_device" });
    }

    return Results.Ok(new
    {
        deviceId = request.DeviceId,
        syncToken = Guid.NewGuid().ToString("N"),
        expiresIn = 3600
    });
});

app.Run();

record TokenRequest(string Username, string Password);
record TokenResponse(string AccessToken, string RefreshToken, int ExpiresIn);
record DeviceRegistrationRequest(Guid DeviceId, string DevicePublicKey, string Platform);

record UserDto(Guid Id, string Email, string FullName, string[] Roles);

record CreateUserCommand(string Email, string FullName, string[] Roles) : IRequest<UserDto>;

class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(3);
    }
}

class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly UserStore _store;

    public CreateUserHandler(UserStore store)
    {
        _store = store;
    }

    public Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new UserDto(Guid.NewGuid(), request.Email, request.FullName, request.Roles);
        _store.Users.Add(user);
        return Task.FromResult(user);
    }
}

class UserStore
{
    public List<UserDto> Users { get; } = new();
}
