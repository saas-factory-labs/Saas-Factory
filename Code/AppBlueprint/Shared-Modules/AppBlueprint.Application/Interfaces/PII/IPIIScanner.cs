using AppBlueprint.SharedKernel.SharedModels.PII;

namespace AppBlueprint.Application.Interfaces.PII;

public interface IPIIScanner
{
    string Name { get; }
    Task<IEnumerable<PIITag>> ScanAsync(string text, CancellationToken cancellationToken = default);
}
