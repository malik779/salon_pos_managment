namespace BranchService.Application.Branches.Models;

public sealed record BranchDto(Guid Id, string Name, string Timezone, string Address);
