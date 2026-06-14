using System.Text.RegularExpressions;

namespace AppBlueprint.AdminPortalKernel.Security;

/// <summary>
/// Validates that a high-sensitivity admin access carries a justification referencing a support
/// ticket (e.g. "Support ticket #12345"), per the insider-threat mitigation. The default
/// implementation validates the format only; a remote check against a ticketing system can be
/// layered on via configuration without changing callers.
/// </summary>
public interface ITicketValidationService
{
    Task<bool> ValidateAsync(string reason, CancellationToken cancellationToken = default);
}

/// <inheritdoc />
public sealed partial class TicketValidationService : ITicketValidationService
{
    [GeneratedRegex(@"#\d+", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 200)]
    private static partial Regex TicketPattern();

    public Task<bool> ValidateAsync(string reason, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(TicketPattern().IsMatch(reason));
    }
}
