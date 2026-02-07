using System.Reflection;
using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.SharedModels;
using AppBlueprint.SharedKernel.SharedModels.PII;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Interceptors;

/// <summary>
/// Interceptor that automatically scans entity properties marked with [PIIRisk] 
/// and populates the Metadata.Pii property before saving.
/// </summary>
public class PiiSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IPIIEngine _piiEngine;

    public PiiSaveChangesInterceptor(IPIIEngine piiEngine)
    {
        _piiEngine = piiEngine;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        if (eventData.Context is null) return result;

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            await ProcessEntryForPII(entry, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task ProcessEntryForPII(EntityEntry entry, CancellationToken cancellationToken)
    {
        if (entry.State is not (EntityState.Added or EntityState.Modified)) return;

        var entityType = entry.Entity.GetType();
        var propertiesToScan = entityType.GetProperties()
            .Where(p => p.GetCustomAttribute<PIIRiskAttribute>() is not null)
            .ToList();

        if (propertiesToScan.Count == 0) return;

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

        if (allTags.Count > 0)
        {
            UpdateEntityMetadata(entry.Entity, allTags);
        }
    }

    private void UpdateEntityMetadata(object entity, List<PIITag> allTags)
    {
        var entityType = entity.GetType();
        var metadataProp = entityType.GetProperty("Metadata");
        if (metadataProp != null && metadataProp.PropertyType == typeof(EntityMetadata))
        {
            var existingMetadata = (EntityMetadata?)metadataProp.GetValue(entity) ?? new EntityMetadata();

            var newMetadata = existingMetadata with
            {
                Pii = new PIIMetadata
                {
                    PiiDetected = true,
                    PiiTags = allTags.DistinctBy(t => new { t.Type, t.Value, t.Start, t.End }).ToList(),
                    ScannerInfo = new ScannerInfo { Version = "1.0", Engine = "PiiSaveChangesInterceptor" }
                }
            };

            metadataProp.SetValue(entity, newMetadata);
        }
    }
}
