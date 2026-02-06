namespace DeploymentManager.ApiService.Domain.Entities;

public class DeploymentEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public bool Active { get; set; }
    public bool Deleted { get; set; }
    public string DeletedBy { get; set; }
    public DateTime DeletedDate { get; set; }
    public string DeploymentType { get; set; }
}
