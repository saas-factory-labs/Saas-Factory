using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Credit;

public class CreditEntity : BaseEntity, ITenantScoped
{
    public decimal CreditRemaining { get; set; }
    public string TenantId { get; set; } = string.Empty;
}
