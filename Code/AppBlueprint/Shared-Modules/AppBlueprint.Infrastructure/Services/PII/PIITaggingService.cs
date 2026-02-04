using System.Threading;
using System.Threading.Tasks;
using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.SharedKernel.SharedModels.PII;
using System.Text.Json;

namespace AppBlueprint.Infrastructure.Services.PII;

public interface IPIITaggingService
{
    Task<string> ProcessTextAndCreateMetadataAsync(string text, string existingMetadataJson = "{}", CancellationToken cancellationToken = default);
}

public class PIITaggingService : IPIITaggingService
{
    private readonly IPIIEngine _piiEngine;

    public PIITaggingService(IPIIEngine piiEngine)
    {
        _piiEngine = piiEngine;
    }

    public async Task<string> ProcessTextAndCreateMetadataAsync(string text, string existingMetadataJson = "{}", CancellationToken cancellationToken = default)
    {
        var piiMetadata = await _piiEngine.ScanAndTagAsync(text, cancellationToken);
        
        // Merge with existing metadata if necessary
        // Assuming existingMetadataJson is a JSON object
        using var doc = JsonDocument.Parse(existingMetadataJson);
        var root = doc.RootElement.Clone();
        
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, WriteIndented = false };
        var piiJson = JsonSerializer.SerializeToElement(piiMetadata, options);
        
        // Use a dictionary to merge properties
        var merged = new Dictionary<string, object>();
        foreach (var prop in root.EnumerateObject())
        {
            merged[prop.Name] = prop.Value;
        }
        
        // Add or overwrite PII fields
        merged["pii_detected"] = piiMetadata.PiiDetected;
        merged["pii_tags"] = piiMetadata.PiiTags;
        merged["scanner_info"] = piiMetadata.ScannerInfo;
        
        return JsonSerializer.Serialize(merged, options);
    }
}
