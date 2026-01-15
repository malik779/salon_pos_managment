using System.Collections.Generic;
using System.Text;
using IdentityService.Api;
using IdentityService.Application.Abstractions;
using IdentityService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Salon.BuildingBlocks.Data;
using Salon.BuildingBlocks.DependencyInjection;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("identity-service");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Service:Name"] = "identity-service"
});

// Configure JWT Authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey is not configured");
var key = Encoding.UTF8.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "salon-pos-management",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "salon-pos-management",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services
    .AddApplicationServices(typeof(Program).Assembly)
    .AddMessagingInfrastructure(builder.Configuration)
    .AddIdentityInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.UseAuthentication();
app.UseAuthorization();

// Seed database
await app.SeedDatabaseAsync<IdentityDbContext>(async (context) =>
{
    var passwordHasher = app.Services.GetRequiredService<IPasswordHasher>();
    var logger = app.Services.GetRequiredService<ILogger<IdentityDbContext>>();
    await IdentityDbSeeder.SeedAsync(context, passwordHasher, logger);
});

app.MapIdentityEndpoints();
app.Run();
