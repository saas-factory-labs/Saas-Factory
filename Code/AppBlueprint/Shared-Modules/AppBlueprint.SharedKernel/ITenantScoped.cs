namespace AppBlueprint.SharedKernel;

public interface ITenantScoped
{
    string TenantId { get; set; }
}
