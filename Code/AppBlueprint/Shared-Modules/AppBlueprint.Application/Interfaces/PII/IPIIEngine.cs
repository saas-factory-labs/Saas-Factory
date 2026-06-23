using AppBlueprint.SharedKernel.SharedModels.PII;

namespace AppBlueprint.Application.Interfaces.PII;

public interface IPiiEngine
{
    Task<PiiMetadata> ScanAndTagAsync(string text, CancellationToken cancellationToken = default);
}
