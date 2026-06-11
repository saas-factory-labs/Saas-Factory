using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Admin;

public class AdminEntity : BaseEntity
{
    public string Email { get; init; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
