namespace ClientService.Application.Clients.Models;

public sealed record ClientDto(Guid Id, string FirstName, string LastName, string Phone, string? Email);
public sealed record ConsentDto(Guid ClientId, string Version, DateTime CapturedAtUtc);
