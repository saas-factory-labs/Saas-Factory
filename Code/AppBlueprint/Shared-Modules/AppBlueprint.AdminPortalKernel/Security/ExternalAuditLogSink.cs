using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppBlueprint.AdminPortalKernel.Security;

/// <summary>Structured admin-access record streamed to an external monitoring system.</summary>
public sealed record AdminAccessAuditPayload(
    string AdminUserId,
    string? AdminEmail,
    string Operation,
    string AppSlug,
    string? TenantId,
    string Reason,
    bool IsAutomatedBypass,
    string? DeviceFingerprint,
    DateTimeOffset OccurredAtUtc);

/// <summary>
/// Streams admin-access audit payloads to an external SIEM (Splunk/DataDog target), per the
/// audit-log-tampering mitigation: a copy of every sensitive access lands somewhere the admin
/// cannot modify. When no endpoint is configured the sink degrades to a structured local log.
/// </summary>
public interface IExternalAuditLogSink
{
    Task EmitAsync(AdminAccessAuditPayload payload, CancellationToken cancellationToken = default);
}

/// <inheritdoc />
public sealed class ExternalAuditLogSink : IExternalAuditLogSink
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AdminPortalSecurityOptions _options;
    private readonly ILogger<ExternalAuditLogSink> _logger;

    public ExternalAuditLogSink(
        IHttpClientFactory httpClientFactory,
        IOptions<AdminPortalSecurityOptions> options,
        ILogger<ExternalAuditLogSink> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task EmitAsync(AdminAccessAuditPayload payload, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        object envelope = BuildEnvelope(payload, _options.Siem.Target);

        if (string.IsNullOrWhiteSpace(_options.Siem.Endpoint))
        {
            // Local fallback: still emit a structured record so the access is never silently dropped.
            _logger.LogInformation("ADMIN_TENANT_ACCESS_SIEM (local) | {Envelope}", JsonSerializer.Serialize(envelope));
            return;
        }

        HttpClient client = _httpClientFactory.CreateClient(nameof(ExternalAuditLogSink));
        using var request = new HttpRequestMessage(HttpMethod.Post, _options.Siem.Endpoint)
        {
            Content = JsonContent.Create(envelope)
        };
        if (!string.IsNullOrWhiteSpace(_options.Siem.ApiKey))
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_options.Siem.ApiKey}");
        }

        try
        {
            using HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            // A SIEM outage must not silently lose the record - surface it in the local log too.
            _logger.LogError(ex, "ADMIN_TENANT_ACCESS_SIEM delivery failed | {Envelope}", JsonSerializer.Serialize(envelope));
            throw;
        }
    }

    /// <summary>
    /// Shapes the payload into the envelope the target system expects. Pure and side-effect free
    /// so the formatting contract can be asserted in unit tests.
    /// </summary>
    internal static object BuildEnvelope(AdminAccessAuditPayload payload, string target)
    {
        ArgumentNullException.ThrowIfNull(payload);

        string normalizedTarget = target is null ? string.Empty : target.Trim();

        if (string.Equals(normalizedTarget, "splunk", StringComparison.OrdinalIgnoreCase))
        {
            return new { sourcetype = "admin_access", time = payload.OccurredAtUtc.ToUnixTimeSeconds(), @event = payload };
        }

        if (string.Equals(normalizedTarget, "datadog", StringComparison.OrdinalIgnoreCase))
        {
            return new { ddsource = "admin_portal", service = "deployment-manager", ddtags = $"app:{payload.AppSlug}", message = payload };
        }

        return new { type = "admin_access", payload };
    }
}
