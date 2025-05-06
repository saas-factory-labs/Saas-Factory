using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

// namespace AppBlueprint.SharedKernel.Models.B2B

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;

public class OrganizationEntity
{
    public OrganizationEntity()
    {
        Customers = new List<CustomerEntity>();
        Teams = new List<TeamEntity>();
    }

    public OrganizationEntity(List<CustomerEntity> customers)
    {
        Customers = customers;
    }

    public int Id { get; set; }

    public required UserEntity Owner { get; set; }

    public required string? Name { get; set; }

    public required string Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastUpdatedAt { get; set; }
    public required List<CustomerEntity> Customers { get; set; }
    public required List<TeamEntity> Teams { get; set; }
}
