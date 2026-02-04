using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.SharedKernel.SharedModels.PII;

namespace AppBlueprint.Infrastructure.Services.PII;

public class NerPIIScannerPlaceholder : IPIIScanner
{
    public string Name => "NER";

    public Task<IEnumerable<PIITag>> ScanAsync(string text, CancellationToken cancellationToken = default)
    {
        // Placeholder for Microsoft Presidio or Microsoft.Recognizers.Text
        return Task.FromResult(Enumerable.Empty<PIITag>());
    }
}

public class LlmPIIScannerPlaceholder : IPIIScanner
{
    public string Name => "LLM";

    public Task<IEnumerable<PIITag>> ScanAsync(string text, CancellationToken cancellationToken = default)
    {
        // Placeholder for LLM-based scanning (e.g. GPT-4o, Gemini, etc.)
        return Task.FromResult(Enumerable.Empty<PIITag>());
    }
}
