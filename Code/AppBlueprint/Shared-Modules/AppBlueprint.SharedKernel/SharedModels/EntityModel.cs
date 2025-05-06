using IdType = System.Guid;

namespace AppBlueprint.SharedKernel.SharedModels;

// IEntityModel<T>
internal class EntityModel : IEntityModel
{
    public DateTime? UpdatedAt { get; set; }
    public IdType Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public bool IsSoftDeleted { get; set; }
}
