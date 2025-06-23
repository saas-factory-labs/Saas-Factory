using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;

public class FileEntity : BaseEntity
{
    public FileEntity()
    {
        Id = PrefixedUlid.Generate("file");
    }

    public string OwnerId { get; set; }

    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public string FileName { get; set; }

    public long FileSize { get; set; }
    public string FileExtension { get; set; }
    public string FilePath { get; set; }
}
