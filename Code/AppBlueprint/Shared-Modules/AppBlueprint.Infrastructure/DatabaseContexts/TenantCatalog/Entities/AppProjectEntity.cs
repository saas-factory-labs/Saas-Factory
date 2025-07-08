using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;

namespace AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog.Entities;

public class AppProjectEntity
{
    public AppProjectEntity()
    {
        Customers = new List<CustomerEntity>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastUpdatedAt { get; set; }

    public string Category { get; set; }

    public string
        DatabaseConnectionStringRef
    { get; set; } // azure keyvault reference to the database connection string

    public string
        DatabaseConnectionStringReference
    {
        get;
        set;
    } // Database that is used for this specific app project (environment variable name) - for example filediscovery-prod-db and environment variable name is FILEDISCOVERY_PROD_DB

    public List<CustomerEntity> Customers { get; set; }
}
