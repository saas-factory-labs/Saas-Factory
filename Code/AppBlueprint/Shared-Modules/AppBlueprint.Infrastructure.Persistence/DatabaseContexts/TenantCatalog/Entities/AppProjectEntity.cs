using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.Customer;

namespace AppBlueprint.Infrastructure.Persistence.DatabaseContexts.TenantCatalog.Entities;

public class AppProjectEntity
{
    public AppProjectEntity()
    {
        Customers = [];
    }

    public int Id { get; set; }
    public required string Name { get; set; }
    public required bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastUpdatedAt { get; set; }

    public string? Category { get; set; }

    public string? DatabaseConnectionStringRef { get; set; } // azure keyvault reference to the database connection string

    public string?
        DatabaseConnectionStringReference
    {
        get;
        set;
    } // Database that is used for this specific app project (environment variable name) - for example filediscovery-prod-db and environment variable name is FILEDISCOVERY_PROD_DB

    public List<CustomerEntity> Customers { get; set; }
}
