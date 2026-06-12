using System.Globalization;
using AppBlueprint.CliKit.Processes;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.CliKit.Runtime;

public sealed record DotNetSdkRequirement(int MajorVersion, string DisplayName);

public interface IDotNetSdkPreflight
{
    Task<bool> IsSdkInstalledAsync(DotNetSdkRequirement requirement, CancellationToken cancellationToken = default);
}

public sealed class DotNetSdkPreflight : IDotNetSdkPreflight
{
    private readonly ICliProcessRunner _processRunner;
    private readonly ILogger<DotNetSdkPreflight> _logger;

    public DotNetSdkPreflight(ICliProcessRunner processRunner, ILogger<DotNetSdkPreflight> logger)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> IsSdkInstalledAsync(DotNetSdkRequirement requirement, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requirement);

        var result = await _processRunner.RunAsync(
            new CliProcessRequest("dotnet", "--list-sdks", StreamOutput: false, Timeout: TimeSpan.FromSeconds(15)),
            cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogDebug("dotnet --list-sdks failed with exit code {ExitCode}.", result.ExitCode);
            return false;
        }

        var sdkVersionPrefix = requirement.MajorVersion.ToString(CultureInfo.InvariantCulture) + ".";

        return result.StandardOutput
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(line => line.StartsWith(sdkVersionPrefix, StringComparison.OrdinalIgnoreCase));
    }
}
