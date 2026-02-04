using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.SharedModels;
using AppBlueprint.SharedKernel.SharedModels.PII;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Interceptors;

/// <summary>
/// Interceptor that automatically scans entity properties marked with [PIIRisk] 
/// and populates the Metadata.Pii property before saving.
/// </summary>
public class PIISaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IPIIEngine _piiEngine;

    public PIISaveChangesInterceptor(IPIIEngine piiEngine)
    {
        _piiEngine = piiEngine;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) return result;

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified)) continue;

            var entityType = entry.Entity.GetType();
            var propertiesToScan = entityType.GetProperties()
                .Where(p => p.GetCustomAttribute<PIIRiskAttribute>() != null)
                .ToList();

            if (!propertiesToScan.Any()) continue;

            var allTags = new List<PIITag>();
            foreach (var prop in propertiesToScan)
            {
                var value = prop.GetValue(entry.Entity)?.ToString();
                if (string.IsNullOrWhiteSpace(value)) continue;

                var scanResult = await _piiEngine.ScanAndTagAsync(value, cancellationToken);
                if (scanResult.PiiDetected)
                {
                    allTags.AddRange(scanResult.PiiTags);
                }
            }

            if (allTags.Any())
            {
                // Find the Metadata property on the entity
                var metadataProp = entityType.GetProperty("Metadata");
                if (metadataProp != null && metadataProp.PropertyType == typeof(EntityMetadata))
                {
                    var existingMetadata = (EntityMetadata?)metadataProp.GetValue(entry.Entity) ?? new EntityMetadata();
                    
                    var newMetadata = existingMetadata with
                    {
                        Pii = new PIIMetadata
                        {
                            PiiDetected = true,
                            PiiTags = allTags.DistinctBy(t => new { t.Type, t.Value, t.Start, t.End }).ToList(),
                            ScannerInfo = new ScannerInfo { Version = "1.0", Engine = "PIISaveChangesInterceptor" }
                        }
                    };

                    metadataProp.SetValue(entry.Entity, newMetadata);
                }
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
