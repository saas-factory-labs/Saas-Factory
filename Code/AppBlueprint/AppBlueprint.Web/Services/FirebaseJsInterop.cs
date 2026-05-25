using Microsoft.JSInterop;

namespace AppBlueprint.Web.Services;

public sealed class FirebaseJsInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FirebaseJsInterop> _logger;
    private bool _initialized;

    public FirebaseJsInterop(
        IJSRuntime jsRuntime,
        IConfiguration configuration,
        ILogger<FirebaseJsInterop> logger)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        var apiKey = _configuration["Authentication:Firebase:ApiKey"];
        var authDomain = _configuration["Authentication:Firebase:AuthDomain"];
        var projectId = _configuration["Authentication:Firebase:ProjectId"];
        var storageBucket = _configuration["Authentication:Firebase:StorageBucket"];
        var messagingSenderId = _configuration["Authentication:Firebase:MessagingSenderId"];
        var appId = _configuration["Authentication:Firebase:AppId"];

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(authDomain) || string.IsNullOrEmpty(projectId))
        {
            _logger.LogWarning("Firebase configuration is incomplete. Firebase sign-in will be unavailable.");
            return;
        }

        await _jsRuntime.InvokeVoidAsync("firebaseAuth.initialize", new
        {
            apiKey,
            authDomain,
            projectId,
            storageBucket,
            messagingSenderId,
            appId
        });

        _initialized = true;
        _logger.LogInformation("Firebase JS interop initialized for project: {ProjectId}", projectId);
    }

    public async Task<FirebaseSignInResult> SignInWithGoogleAsync()
    {
        if (!_initialized)
            await InitializeAsync();

        if (!_initialized)
            return new FirebaseSignInResult { Success = false, Error = "Firebase is not configured" };

        try
        {
            return await _jsRuntime.InvokeAsync<FirebaseSignInResult>("firebaseAuth.signInWithGoogle");
        }
        catch (JSException ex)
        {
            _logger.LogError(ex, "Firebase Google sign-in JS interop failed");
            return new FirebaseSignInResult { Success = false, Error = "Sign-in failed: " + ex.Message };
        }
    }

    public async Task<FirebaseExchangeResult> ExchangeIdTokenAsync(string idToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(idToken);
        try
        {
            return await _jsRuntime.InvokeAsync<FirebaseExchangeResult>("firebaseAuth.exchangeIdToken", idToken);
        }
        catch (JSException ex)
        {
            _logger.LogError(ex, "Firebase ID token exchange JS interop failed");
            return new FirebaseExchangeResult { Success = false, Error = "Token exchange failed: " + ex.Message };
        }
    }

    public async Task SignOutAsync()
    {
        if (!_initialized) return;
        await _jsRuntime.InvokeVoidAsync("firebaseAuth.signOut");
    }

    public bool IsConfigured =>
        !string.IsNullOrEmpty(_configuration["Authentication:Firebase:ProjectId"]);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

public sealed class FirebaseSignInResult
{
    public bool Success { get; set; }
    public string? IdToken { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? Error { get; set; }
}

public sealed class FirebaseExchangeResult
{
    public bool Success { get; set; }
    public string? RedirectTo { get; set; }
    public string? Error { get; set; }
}
