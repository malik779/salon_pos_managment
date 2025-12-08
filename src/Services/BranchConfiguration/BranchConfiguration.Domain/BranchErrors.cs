using BuildingBlocks.Domain.Primitives;

namespace BranchConfiguration.Domain;

public static class BranchErrors
{
    public static readonly Error NotFound = new("BranchConfiguration.BranchNotFound", "Branch not found");
}
