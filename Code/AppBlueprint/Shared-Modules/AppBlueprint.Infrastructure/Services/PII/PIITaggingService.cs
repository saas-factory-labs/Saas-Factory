using System.Collections.Generic;
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
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = false,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

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

        // Check for non-canonical tags to potentially notify the Worker
        foreach (var tag in piiMetadata.PiiTags)
        {
            if (!tag.IsCanonical)
            {
                // TODO: In a real scenario, we would push to a Queue/Worker here
                // For now, we'll just log or mark it
                Console.WriteLine($"[PII Discovery] Found non-canonical label: {tag.Type}");
            }
        }
        
        // Use a dictionary to merge properties
        var merged = new Dictionary<string, object>();
        foreach (var prop in root.EnumerateObject())
        {
            merged[prop.Name] = prop.Value;
        }
        
        // Add or overwrite PII fields
        merged["PiiDetected"] = piiMetadata.PiiDetected;
        merged["PiiTags"] = piiMetadata.PiiTags;
        merged["ScannerInfo"] = piiMetadata.ScannerInfo;
        
        return JsonSerializer.Serialize(merged, _jsonSerializerOptions);
    }
}
