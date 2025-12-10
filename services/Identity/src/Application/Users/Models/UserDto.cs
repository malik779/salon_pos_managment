namespace IdentityService.Application.Users.Models;

public sealed record UserDto(Guid Id, string Email, string FullName, string[] Roles);
