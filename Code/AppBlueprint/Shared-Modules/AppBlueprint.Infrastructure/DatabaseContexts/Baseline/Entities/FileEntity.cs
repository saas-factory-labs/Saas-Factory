using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;

public class FileEntity
{
    public int Id { get; set; }
    public int OwnerId { get; set; }

    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public string FileName { get; set; }

    public long FileSize { get; set; }
    public string FileExtension { get; set; }
    public string FilePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
