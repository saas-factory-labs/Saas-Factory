using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities;

public class LanguageEntity : BaseEntity
{
    public LanguageEntity()
    {
        Id = PrefixedUlid.Generate("lang");
    }

    public required string Name { get; set; }

    public required string Code { get; set; }
}
