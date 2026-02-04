using System.Threading;
using System.Threading.Tasks;
using AppBlueprint.SharedKernel.SharedModels.PII;

namespace AppBlueprint.Application.Interfaces.PII;

public interface IPIIEngine
{
    Task<PIIMetadata> ScanAndTagAsync(string text, CancellationToken cancellationToken = default);
}
