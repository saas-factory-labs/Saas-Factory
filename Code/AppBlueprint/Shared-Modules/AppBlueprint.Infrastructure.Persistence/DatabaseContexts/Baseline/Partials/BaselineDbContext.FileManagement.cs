using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.EntityConfigurations;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.FileManagement;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline;

public partial class BaselineDbContext
{
    public DbSet<FileMetadataEntity> FileMetadata { get; set; }

    partial void OnModelCreating_FileManagement(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfiguration(new FileMetadataConfiguration());
    }
}
