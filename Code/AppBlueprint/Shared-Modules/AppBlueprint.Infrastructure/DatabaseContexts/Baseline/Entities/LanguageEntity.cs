using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class LanguageEntity: BaseEntity
{
    public required string Name { get; set; }

    public required string Code { get; set; }
}
