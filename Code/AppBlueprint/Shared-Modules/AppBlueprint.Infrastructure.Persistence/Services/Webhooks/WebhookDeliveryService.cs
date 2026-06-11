using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Services.Webhooks;

public class WebhookDeliveryService : IWebhookDeliveryService
{
    private const int MaxRetries = 3;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);

    private readonly IWebhookRepository _webhookRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhookDeliveryService> _logger;

    public WebhookDeliveryService(
        IWebhookRepository webhookRepository,
        IHttpClientFactory httpClientFactory,
        ILogger<WebhookDeliveryService> logger)
    {
        ArgumentNullException.ThrowIfNull(webhookRepository);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(logger);

        _webhookRepository = webhookRepository;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task DeliverAsync(string eventType, object payload, string tenantId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventType);
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(tenantId);

        IEnumerable<DatabaseContexts.Baseline.Entities.WebhookEntity> endpoints =
            await _webhookRepository.GetByTenantIdAsync(tenantId, cancellationToken);

        IEnumerable<DatabaseContexts.Baseline.Entities.WebhookEntity> matching = endpoints.Where(e =>
            string.IsNullOrEmpty(e.EventTypes) ||
            e.EventTypes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Contains(eventType, StringComparer.Ordinal));

        string payloadJson = JsonSerializer.Serialize(payload);

        foreach (DatabaseContexts.Baseline.Entities.WebhookEntity endpoint in matching)
        {
            await DeliverToEndpointAsync(endpoint, eventType, payloadJson, cancellationToken);
        }
    }

    private async Task DeliverToEndpointAsync(
        DatabaseContexts.Baseline.Entities.WebhookEntity endpoint,
        string eventType,
        string payloadJson,
        CancellationToken cancellationToken)
    {
        string signature = ComputeHmacSha256Signature(endpoint.Secret, payloadJson);
        using HttpClient client = _httpClientFactory.CreateClient("WebhookDelivery");

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, endpoint.Url);
                request.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
                request.Headers.TryAddWithoutValidation("X-Webhook-Signature", $"sha256={signature}");
                request.Headers.TryAddWithoutValidation("X-Webhook-Event", eventType);

                HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Webhook {EventType} delivered to {Url} (attempt {Attempt})",
                        eventType, endpoint.Url, attempt);
                    return;
                }

                _logger.LogWarning(
                    "Webhook {EventType} delivery to {Url} failed with status {Status} (attempt {Attempt}/{MaxRetries})",
                    eventType, endpoint.Url, (int)response.StatusCode, attempt, MaxRetries);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex,
                    "Webhook {EventType} delivery to {Url} threw exception (attempt {Attempt}/{MaxRetries})",
                    eventType, endpoint.Url, attempt, MaxRetries);
            }

            if (attempt < MaxRetries)
            {
                await Task.Delay(RetryDelay, cancellationToken);
            }
        }

        _logger.LogError(
            "Webhook {EventType} delivery to {Url} failed after {MaxRetries} attempts",
            eventType, endpoint.Url, MaxRetries);
    }

    private static string ComputeHmacSha256Signature(string secret, string payload)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        byte[] hashBytes = HMACSHA256.HashData(keyBytes, payloadBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
