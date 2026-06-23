using AppBlueprint.SharedKernel.SharedModels.PII;

namespace AppBlueprint.Application.Interfaces.PII;

public interface IPiiScanner
{
    string Name { get; }
    Task<IEnumerable<PiiTag>> ScanAsync(string text, CancellationToken cancellationToken = default);
}
