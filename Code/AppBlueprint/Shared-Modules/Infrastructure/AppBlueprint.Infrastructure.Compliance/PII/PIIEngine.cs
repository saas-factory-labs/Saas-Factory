using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.SharedKernel.SharedModels.PII;

namespace AppBlueprint.Infrastructure.Compliance.PII;

public class PiiEngine : IPiiEngine
{
    private readonly IEnumerable<IPiiScanner> _scanners;

    public PiiEngine(IEnumerable<IPiiScanner> scanners)
    {
        _scanners = scanners;
    }

    public async Task<PiiMetadata> ScanAndTagAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new PiiMetadata
            {
                PiiDetected = false,
                ScannerInfo = new ScannerInfo { Engine = GetEngineSummary() }
            };
        }

        List<PiiTag> allTags = [];
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

        return new PiiMetadata
        {
            PiiDetected = distinctTags.Count > 0,
            PiiTags = distinctTags,
            ScannerInfo = new ScannerInfo { Engine = GetEngineSummary() }
        };
    }

    private string GetEngineSummary()
    {
        return string.Join("+", _scanners.Select(s => s.Name));
    }
}
