using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.SharedKernel.SharedModels.PII;

namespace AppBlueprint.Infrastructure.Compliance.PII;

public class NerPiiScannerPlaceholder : IPiiScanner
{
    public string Name => "NER";

    public Task<IEnumerable<PiiTag>> ScanAsync(string text, CancellationToken cancellationToken = default)
    {
        // Placeholder for Microsoft Presidio or Microsoft.Recognizers.Text
        return Task.FromResult(Enumerable.Empty<PiiTag>());
    }
}

public class LlmPiiScannerPlaceholder : IPiiScanner
{
    public string Name => "LLM";

    public Task<IEnumerable<PiiTag>> ScanAsync(string text, CancellationToken cancellationToken = default)
    {
        // Placeholder for LLM-based scanning (e.g. GPT-4o, Gemini, etc.)
        return Task.FromResult(Enumerable.Empty<PiiTag>());
    }
}
