using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Authorization.Providers.Firebase;

public class FirebaseProvider : BaseAuthenticationProvider
{
    private readonly FirebaseConfiguration _configuration;
    private readonly ILogger<FirebaseProvider> _logger;
    private FirebaseAuth? _firebaseAuth;

    public FirebaseProvider(
        ITokenStorageService tokenStorage,
        IConfiguration configuration,
        ILogger<FirebaseProvider> logger) : base(tokenStorage)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _configuration = new FirebaseConfiguration();
        configuration.GetSection("Authentication:Firebase").Bind(_configuration);

        ValidateConfiguration();
        InitializeFirebaseApp();
    }

    public override async Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Firebase authentication is handled client-side (via JS SDK) and then
        // the ID token is exchanged via the /auth/firebase/callback endpoint.
        // This method supports email/password sign-in for API clients using the REST API.
        _logger.LogWarning(
            "Direct Firebase login via LoginAsync is not supported. " +
            "Use the Firebase JS SDK client-side and exchange the ID token via /auth/firebase/callback.");

        return await Task.FromResult(new AuthenticationResult
        {
            IsSuccess = false,
            Error = "Firebase authentication must be initiated client-side via the Firebase JS SDK."
        });
    }

    public override async Task<AuthenticationResult> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(AccessToken))
        {
            return new AuthenticationResult { IsSuccess = false, Error = "No token available" };
        }

        try
        {
            // Firebase tokens are short-lived (1 hour); refresh is handled client-side
            // Verify the current token is still valid
            if (_firebaseAuth is null)
            {
                return new AuthenticationResult { IsSuccess = false, Error = "Firebase not initialized" };
            }

            var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(AccessToken, cancellationToken);
            var expiry = DateTimeOffset.FromUnixTimeSeconds(decodedToken.ExpirationTimeSeconds).UtcDateTime;

            return new AuthenticationResult
            {
                IsSuccess = true,
                AccessToken = AccessToken,
                ExpiresAt = expiry
            };
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogWarning(ex, "Firebase token validation failed during refresh");
            return new AuthenticationResult { IsSuccess = false, Error = "Firebase token is invalid or expired" };
        }
    }

    public async Task<AuthenticationResult> VerifyIdTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(idToken);

        if (_firebaseAuth is null)
        {
            return new AuthenticationResult { IsSuccess = false, Error = "Firebase not initialized" };
        }

        try
        {
            var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(idToken, cancellationToken);
            var expiry = DateTimeOffset.FromUnixTimeSeconds(decodedToken.ExpirationTimeSeconds).UtcDateTime;

            var result = new AuthenticationResult
            {
                IsSuccess = true,
                AccessToken = idToken,
                ExpiresAt = expiry,
                AdditionalData = new Dictionary<string, object>
                {
                    ["uid"] = decodedToken.Uid,
                    ["email"] = decodedToken.Claims.GetValueOrDefault("email") ?? string.Empty,
                    ["email_verified"] = decodedToken.Claims.GetValueOrDefault("email_verified") ?? false,
                    ["name"] = decodedToken.Claims.GetValueOrDefault("name") ?? string.Empty,
                    ["picture"] = decodedToken.Claims.GetValueOrDefault("picture") ?? string.Empty
                }
            };

            await StoreTokens(result);
            return result;
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogWarning(ex, "Firebase ID token verification failed");
            return new AuthenticationResult { IsSuccess = false, Error = "Invalid or expired Firebase ID token" };
        }
    }

    protected override async Task TryRestoreFromStoredToken(string storedToken)
    {
        try
        {
            var result = await VerifyIdTokenAsync(storedToken);
            if (result.IsSuccess)
            {
                AccessToken = storedToken;
                TokenExpiration = result.ExpiresAt ?? DateTime.UtcNow.AddHours(1);
                NotifyAuthenticationStateChanged();
            }
            else
            {
                await TokenStorage.RemoveTokenAsync();
            }
        }
#pragma warning disable CA1031
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to restore Firebase token from storage");
            await TokenStorage.RemoveTokenAsync();
        }
#pragma warning restore CA1031
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_configuration.ProjectId))
            throw new InvalidOperationException("Firebase ProjectId is required in Authentication:Firebase configuration");

        if (string.IsNullOrEmpty(_configuration.ApiKey))
            throw new InvalidOperationException("Firebase ApiKey is required in Authentication:Firebase configuration");
    }

    private void InitializeFirebaseApp()
    {
        try
        {
            var app = FirebaseApp.DefaultInstance
                ?? FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault(),
                    ProjectId = _configuration.ProjectId
                });

            _firebaseAuth = FirebaseAuth.GetAuth(app);
            _logger.LogInformation("Firebase app initialized for project: {ProjectId}", _configuration.ProjectId);
        }
#pragma warning disable CA1031
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Firebase app initialization failed. Server-side token verification will be unavailable. " +
                "Ensure GOOGLE_APPLICATION_CREDENTIALS is set or a service account is configured.");
        }
#pragma warning restore CA1031
    }
}
