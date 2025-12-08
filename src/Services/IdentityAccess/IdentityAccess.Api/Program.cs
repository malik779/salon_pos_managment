using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Infrastructure;
using BuildingBlocks.Infrastructure.Logging;
using BuildingBlocks.Messaging;
using IdentityAccess.Application.Users.Commands.CreateUser;
using IdentityAccess.Application.Users.Queries.GetUsers;
using IdentityAccess.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSalonSerilog("IdentityAccess.Api");

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Identity & Access API",
            Version = "v1"
        });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }, new List<string>()
            }
        });
    });

builder.Services.AddProblemDetails();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddAuthorization();

var assemblies = new[] { typeof(CreateUserCommand).Assembly };
builder.Services.AddIdentityAccessInfrastructure(builder.Configuration);
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddInfrastructureBuildingBlocks(builder.Configuration, assemblies);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGroup("/api/v1/users")
    .RequireAuthorization()
    .MapUserEndpoints();

app.Run();

static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("",
            async (CreateUserCommand command, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.IsSuccess
                    ? Results.Created($"/api/v1/users/{result.Value}", new { id = result.Value })
                    : Results.BadRequest(new { error = result.Error.Message });
            })
            .WithName("CreateUser")
            .WithOpenApi();

        group.MapGet("",
            async (int pageNumber, int pageSize, ISender sender, CancellationToken ct) =>
            {
                var query = new GetUsersQuery(pageNumber, pageSize);
                var result = await sender.Send(query, ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error.Message });
            })
            .WithName("GetUsers")
            .WithOpenApi();

        return group;
    }
}
