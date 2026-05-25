using AppBlueprint.Application.Interfaces;
using AppBlueprint.Domain.Entities.Notifications;
using AppBlueprint.Domain.Interfaces.Repositories;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Services.Notifications;

/// <summary>
/// Service for sending push notifications via Firebase Cloud Messaging (FCM).
/// </summary>
public sealed class FirebasePushNotificationService : IPushNotificationService
{
    private readonly IPushNotificationTokenRepository _tokenRepository;
    private readonly ILogger<FirebasePushNotificationService> _logger;
    private FirebaseApp? _firebaseApp;
    private readonly object _lock = new();
    private readonly IConfiguration _configuration;
    private bool _isInitialized;

    public FirebasePushNotificationService(
        IPushNotificationTokenRepository tokenRepository,
        ILogger<FirebasePushNotificationService> logger,
        IConfiguration configuration)
    {
        _tokenRepository = tokenRepository;
        _logger = logger;
        _configuration = configuration;
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        try
        {
            if (_firebaseApp is not null)
            {
                _isInitialized = true;
                return;
            }

            lock (_lock)
            {
                if (_firebaseApp is not null)
                {
                    _isInitialized = true;
                    return;
                }

                string? credentialsJson = _configuration["Firebase:CredentialsJson"];
                string? projectId = _configuration["Firebase:ProjectId"];

                if (string.IsNullOrWhiteSpace(credentialsJson) || string.IsNullOrWhiteSpace(projectId))
                {
                    _logger.LogWarning("Firebase credentials not configured. Push notifications will be disabled.");
                    _isInitialized = false;
                    return;
                }

                _firebaseApp = FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromJson(credentialsJson),
                    ProjectId = projectId
                });

                _isInitialized = true;
                _logger.LogInformation("Firebase initialized successfully for project {ProjectId}", projectId);
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid Firebase configuration - check credentials JSON and project ID");
            _isInitialized = false;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Firebase already initialized or invalid operation");
            _isInitialized = false;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Failed to read Firebase credentials");
            _isInitialized = false;
        }
    }

    public async Task SendAsync(PushNotificationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!_isInitialized)
        {
            _logger.LogWarning("Firebase not initialized. Cannot send notification for user {UserId}", request.UserId);
            return;
        }

        List<PushNotificationTokenEntity> tokens = await _tokenRepository.GetActiveByUserIdAsync(request.UserId);

        _logger.LogInformation("Found {Count} active tokens for user {UserId}", tokens.Count, request.UserId);

        if (tokens.Count == 0)
        {
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            foreach (var token in tokens)
            {
                _logger.LogDebug("Sending to token: {Token}", string.Concat(token.Token.AsSpan(0, Math.Min(token.Token.Length, 10)), "..."));
            }
        }

        MulticastMessage message = BuildFcmMessage(
            request.Title,
            request.Body,
            request.ImageUrl,
            request.ActionUrl,
            request.Data,
            tokens.Select(t => t.Token).ToList());

        BatchResponse response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

        _logger.LogInformation("FCM batch send completed. Success: {SuccessCount}, Failure: {FailureCount}",
            response.SuccessCount, response.FailureCount);

        await HandleFailedTokensAsync(response, tokens);
    }

    public async Task<int> SendToTenantAsync(string tenantId, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        if (!_isInitialized)
        {
            _logger.LogWarning("Firebase not initialized. Cannot send notification to tenant {TenantId}", tenantId);
            return 0;
        }

        List<PushNotificationTokenEntity> tokens = await _tokenRepository.GetActiveByTenantIdAsync(tenantId, cancellationToken);

        if (tokens.Count == 0)
        {
            _logger.LogInformation("No active push tokens found for tenant {TenantId}", tenantId);
            return 0;
        }

        MulticastMessage message = BuildFcmMessage(
            title,
            body,
            null, // No image for now
            null, // No action URL for now
            data,
            tokens.Select(t => t.Token).ToList());

        BatchResponse response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, cancellationToken);

        _logger.LogInformation("FCM tenant batch send completed. Success: {SuccessCount}, Failure: {FailureCount}",
            response.SuccessCount, response.FailureCount);

        await HandleFailedTokensAsync(response, tokens);

        return response.SuccessCount;
    }

    public async Task RegisterTokenAsync(RegisterPushTokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        PushNotificationTokenEntity? existingToken = await _tokenRepository.GetByTokenAsync(request.Token);

        if (existingToken is not null)
        {
            if (!existingToken.IsActive)
            {
                existingToken.Reactivate();
                await _tokenRepository.UpdateAsync(existingToken);
            }
            else
            {
                existingToken.UpdateLastUsed();
                await _tokenRepository.UpdateAsync(existingToken);
            }
        }
        else
        {
            PushNotificationTokenEntity token = PushNotificationTokenEntity.Create(
                request.TenantId,
                request.UserId,
                request.Token,
                request.Platform);

            await _tokenRepository.AddAsync(token);
        }

        _logger.LogInformation("Registered push token for user {UserId} on platform {Platform}",
            request.UserId, request.Platform);
    }

    public async Task UnregisterTokenAsync(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        await _tokenRepository.DeactivateByTokenAsync(token);

        _logger.LogInformation("Unregistered push token");
    }

    private MulticastMessage BuildFcmMessage(
        string title,
        string body,
        Uri? imageUrl,
        Uri? actionUrl,
        Dictionary<string, string>? data,
        List<string> tokens)
    {
        Notification notification = new()
        {
            Title = title,
            Body = body,
            ImageUrl = imageUrl?.ToString()
        };

        data ??= new Dictionary<string, string>();
        if (actionUrl is not null)
        {
            data["clickAction"] = actionUrl.ToString();
        }

        return new MulticastMessage
        {
            Tokens = tokens,
            Notification = notification,
            Data = data,
            Webpush = new WebpushConfig
            {
                Notification = new WebpushNotification
                {
                    Title = title,
                    Body = body,
                    Icon = imageUrl?.ToString()
                },
                FcmOptions = new WebpushFcmOptions
                {
                    Link = actionUrl?.ToString()
                }
            },
            Apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    MutableContent = true,
                    Sound = "default"
                }
            }
        };
    }

    private async Task HandleFailedTokensAsync(BatchResponse response, List<PushNotificationTokenEntity> tokens)
    {
        if (response.FailureCount == 0)
            return;

        for (int i = 0; i < response.Responses.Count; i++)
        {
            SendResponse sendResponse = response.Responses[i];
            if (sendResponse.IsSuccess)
                continue;

            string? errorCode = sendResponse.Exception?.MessagingErrorCode?.ToString();
            string? errorMessage = sendResponse.Exception?.Message;

            _logger.LogWarning("FCM token failure: {ErrorCode} - {ErrorMessage}", errorCode, errorMessage);

            if (errorCode is "InvalidArgument" or "Unregistered" or "SenderIdMismatch")
            {
                PushNotificationTokenEntity token = tokens[i];
                token.Deactivate();
                await _tokenRepository.UpdateAsync(token);

                _logger.LogWarning("Deactivated invalid push token due to error: {ErrorCode}", errorCode);
            }
        }
    }
}
