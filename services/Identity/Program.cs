using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { service = "identity", status = "ok" }));

app.MapPost("/auth/token", ([FromBody] TokenRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new { error = "invalid_credentials" });
    }

    var response = new TokenResponse(
        AccessToken: $"fake-token-for-{request.Username}",
        RefreshToken: Guid.NewGuid().ToString("N"),
        ExpiresIn: 3600
    );

    return Results.Ok(response);
});

app.MapPost("/users", ([FromBody] UserRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Email))
    {
        return Results.BadRequest(new { error = "email_required" });
    }

    var user = new
    {
        id = Guid.NewGuid(),
        request.Email,
        request.FullName,
        roles = request.Roles ?? Array.Empty<string>()
    };

    return Results.Created($"/users/{user.id}", user);
});

app.Run();

record TokenRequest(string Username, string Password);
record TokenResponse(string AccessToken, string RefreshToken, int ExpiresIn);
record UserRequest(string Email, string FullName, string[]? Roles);
