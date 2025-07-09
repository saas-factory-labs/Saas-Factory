using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.Organization;

public class OrganizationEntity
{
    public int Id { get; set; }

    public UserEntity? Owner { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastUpdatedAt { get; set; }
    public required List<CustomerEntity> Customers { get; set; } = new List<CustomerEntity>();
    public List<TeamEntity>? Teams { get; set; } = new List<TeamEntity>();
}
