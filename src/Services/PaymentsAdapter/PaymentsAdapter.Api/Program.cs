using BuildingBlocks.Infrastructure;
using BuildingBlocks.Infrastructure.Logging;
using BuildingBlocks.Messaging;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using PaymentsAdapter.Application.Payments.Commands.CapturePayment;
using PaymentsAdapter.Application.Payments.Queries.GetPaymentStatus;
using PaymentsAdapter.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSalonSerilog("PaymentsAdapter.Api");

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Payments Adapter API", Version = "v1" });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
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

var assemblies = new[] { typeof(CapturePaymentCommand).Assembly };

builder.Services.AddPaymentsInfrastructure(builder.Configuration);
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

app.MapGroup("/api/v1/payments")
    .RequireAuthorization()
    .MapPaymentEndpoints();

app.Run();

static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("", async (CapturePaymentCommand command, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.IsSuccess
                    ? Results.Created($"/api/v1/payments/{result.Value}", new { id = result.Value })
                    : Results.BadRequest(new { error = result.Error.Message });
            })
            .WithName("CapturePayment")
            .WithOpenApi();

        group.MapGet("", async ([AsParameters] GetPaymentStatusQuery query, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(query, ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(new { error = result.Error.Message });
            })
            .WithName("GetPaymentStatus")
            .WithOpenApi();

        return group;
    }
}
