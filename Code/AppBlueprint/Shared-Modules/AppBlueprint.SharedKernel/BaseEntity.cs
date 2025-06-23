using System;

namespace AppBlueprint.SharedKernel;

public abstract class BaseEntity : IEntity
{
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedAt { get; set; }
    public bool IsSoftDeleted { get; set; }

    protected BaseEntity()
    {
        // Id will be set by derived classes using PrefixedUlid.Generate()
    }
}