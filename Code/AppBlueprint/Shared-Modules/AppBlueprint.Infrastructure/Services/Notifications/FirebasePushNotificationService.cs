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
public sealed class FirebasePushNotificationService : IFirebaseNotificationService
{
    private readonly IPushNotificationTokenRepository _tokenRepository;
    private readonly ILogger<FirebasePushNotificationService> _logger;
    private static FirebaseApp? _firebaseApp;
    private static readonly object _lock = new();
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase");
            _isInitialized = false;
        }
    }

    public async Task<bool> IsConfiguredAsync()
    {
        return await Task.FromResult(_isInitialized);
    }

    public async Task<int> SendToUserAsync(
        string userId,
        string title,
        string body,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
        {
            _logger.LogWarning("Firebase not initialized. Cannot send notification to user {UserId}", userId);
            return 0;
        }

        var tokens = await _tokenRepository.GetActiveByUserIdAsync(userId, cancellationToken);

        if (tokens.Count == 0)
        {
            _logger.LogInformation("No active tokens found for user {UserId}", userId);
            return 0;
        }

        var tokenStrings = tokens.Select(t => t.Token).ToList();
        return await SendToTokensAsync(tokenStrings, title, body, data, cancellationToken);
    }
    
    public async Task<bool> SendToTokenAsync(
        string token,
        string title,
        string body,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
        {
            _logger.LogWarning("Firebase not initialized. Cannot send notification");
            return false;
        }

        try
        {
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data ?? new Dictionary<string, string>()
            };

            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
            _logger.LogInformation("Successfully sent notification to token. Response: {Response}", response);
            return true;
        }
        catch (FirebaseMessagingException ex) when (ex.MessagingErrorCode == MessagingErrorCode.Unregistered)
        {
            _logger.LogWarning("Token is invalid or unregistered: {Token}. Deactivating token.", token);
            await DeactivateTokenAsync(token);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to token {Token}", token);
            return false;
        }
    }
    
    public async Task<int> SendToTokensAsync(
        IEnumerable<string> tokens,
        string title,
        string body,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
        {
            _logger.LogWarning("Firebase not initialized. Cannot send notifications");
            return 0;
        }

        var tokenList = tokens.ToList();
        if (tokenList.Count == 0)
        {
            return 0;
        }

        try
        {
            var message = new MulticastMessage
            {
                Tokens = tokenList,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data ?? new Dictionary<string, string>()
            };

            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, cancellationToken);

            _logger.LogInformation(
                "Sent multicast notification. Success: {SuccessCount}, Failure: {FailureCount}",
                response.SuccessCount,
                response.FailureCount);

            if (response.FailureCount > 0)
            {
                for (var i = 0; i < response.Responses.Count; i++)
                {
                    var sendResponse = response.Responses[i];
                    if (!sendResponse.IsSuccess && 
                        sendResponse.Exception is FirebaseMessagingException fmEx &&
                        fmEx.MessagingErrorCode == MessagingErrorCode.Unregistered)
                    {
                        var invalidToken = tokenList[i];
                        _logger.LogWarning("Deactivating invalid token: {Token}", invalidToken);
                        await DeactivateTokenAsync(invalidToken);
                    }
                }
            }

            return response.SuccessCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send multicast notification");
            return 0;
        }
    }
    
    public async Task<int> SendToTenantAsync(
        string tenantId,
        string title,
        string body,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
        {
            _logger.LogWarning("Firebase not initialized. Cannot send notification to tenant {TenantId}", tenantId);
            return 0;
        }

        var tokens = await _tokenRepository.GetActiveByTenantIdAsync(tenantId, cancellationToken);

        if (tokens.Count == 0)
        {
            _logger.LogInformation("No active tokens found for tenant {TenantId}", tenantId);
            return 0;
        }

        var tokenStrings = tokens.Select(t => t.Token).ToList();
        return await SendToTokensAsync(tokenStrings, title, body, data, cancellationToken);
    }

    private async Task DeactivateTokenAsync(string token)
    {
        try
        {
            var tokenEntity = await _tokenRepository.GetByTokenAsync(token);
            if (tokenEntity != null)
            {
                tokenEntity.Deactivate();
                await _tokenRepository.UpdateAsync(tokenEntity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deactivate token {Token}", token);
        }
    }
}
