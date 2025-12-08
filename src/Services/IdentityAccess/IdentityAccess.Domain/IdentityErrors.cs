using BuildingBlocks.Domain.Primitives;

namespace IdentityAccess.Domain;

public static class IdentityErrors
{
    public static readonly Error DuplicateEmail = new("Identity.DuplicateEmail", "A user with the supplied email already exists.");
    public static readonly Error UserNotFound = new("Identity.UserNotFound", "User not found.");
}
