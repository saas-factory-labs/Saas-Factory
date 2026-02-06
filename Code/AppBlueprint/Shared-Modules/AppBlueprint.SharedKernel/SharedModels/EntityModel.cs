namespace AppBlueprint.SharedKernel.SharedModels;

// IEntityModel<T>
internal class EntityModel : IEntityModel
{
    public EntityModel()
    {
        Id = PrefixedUlid.Generate("entity");
    }

    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public bool IsSoftDeleted { get; set; }
}
