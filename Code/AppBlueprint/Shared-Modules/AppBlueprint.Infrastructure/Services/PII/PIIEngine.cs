using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.SharedKernel.SharedModels.PII;

namespace AppBlueprint.Infrastructure.Services.PII;

public class PIIEngine : IPIIEngine
{
    private readonly IEnumerable<IPIIScanner> _scanners;

    public PIIEngine(IEnumerable<IPIIScanner> scanners)
    {
        _scanners = scanners;
    }

    public async Task<PIIMetadata> ScanAndTagAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new PIIMetadata
            {
                PiiDetected = false,
                ScannerInfo = new ScannerInfo { Engine = GetEngineSummary() }
            };
        }

        var allTags = new List<PIITag>();
        foreach (var scanner in _scanners)
        {
            var results = await scanner.ScanAsync(text, cancellationToken);
            allTags.AddRange(results);
        }

        // De-duplicate or merge overlapping tags if needed
        // For now, just take all unique findings
        var distinctTags = allTags
            .GroupBy(t => new { t.Start, t.End, t.Type, t.Value })
            .Select(g => g.First())
            .ToList();

        return new PIIMetadata
        {
            PiiDetected = distinctTags.Any(),
            PiiTags = distinctTags,
            ScannerInfo = new ScannerInfo { Engine = GetEngineSummary() }
        };
    }

    private string GetEngineSummary()
    {
        return string.Join("+", _scanners.Select(s => s.Name));
    }
}
