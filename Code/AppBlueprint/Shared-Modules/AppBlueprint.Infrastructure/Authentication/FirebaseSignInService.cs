using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Authentication;

/// <summary>
/// Lightweight service for authenticating a user against Firebase Identity Toolkit.
/// Does not depend on IJSRuntime/ITokenStorageService, so it can be used in regular HTTP
/// request contexts (e.g. MVC controllers) as well as Blazor circuits.
/// </summary>
public interface IFirebaseSignInService
{
    Task<FirebaseSignInResult> SignInAsync(string email, string password, CancellationToken cancellationToken = default);
}

public sealed class FirebaseSignInResult
{
    public bool IsSuccess { get; init; }
    public string? IdToken { get; init; }
    public string? Uid { get; init; }
    public string? Email { get; init; }
    public string? Error { get; init; }
}

public sealed class FirebaseSignInService : IFirebaseSignInService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FirebaseSignInService> _logger;

    private const string SignInUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public FirebaseSignInService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<FirebaseSignInService> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<FirebaseSignInResult> SignInAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentException.ThrowIfNullOrEmpty(password);

        string? apiKey = Environment.GetEnvironmentVariable("FIREBASE_API_KEY")
                      ?? _configuration["Authentication:Firebase:ApiKey"];

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("Firebase API key is not configured. Set FIREBASE_API_KEY environment variable.");
            return new FirebaseSignInResult { IsSuccess = false, Error = "Authentication service is not configured." };
        }

        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var payload = new { email, password, returnSecureToken = true };
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestUri = new Uri($"{SignInUrl}?key={apiKey}", UriKind.Absolute);
            var response = await httpClient.PostAsync(requestUri, content, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                string? errorMessage = TryParseFirebaseError(responseBody);
                _logger.LogWarning("Firebase sign-in failed for {Email}: {Error}", email, errorMessage);
                return new FirebaseSignInResult { IsSuccess = false, Error = errorMessage ?? "Invalid email or password." };
            }

            var body = JsonSerializer.Deserialize<FirebaseIdentityResponse>(responseBody, JsonOptions);

            if (body is null || string.IsNullOrEmpty(body.IdToken))
            {
                _logger.LogError("Firebase returned unexpected response for {Email}", email);
                return new FirebaseSignInResult { IsSuccess = false, Error = "Unexpected response from authentication service." };
            }

            return new FirebaseSignInResult
            {
                IsSuccess = true,
                IdToken = body.IdToken,
                Uid = body.LocalId,
                Email = body.Email
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during Firebase sign-in for {Email}", email);
            return new FirebaseSignInResult { IsSuccess = false, Error = "A network error occurred. Please check your connection." };
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Firebase sign-in request timed out for {Email}", email);
            return new FirebaseSignInResult { IsSuccess = false, Error = "The request timed out. Please try again." };
        }
    }

    private static string? TryParseFirebaseError(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            string? message = doc.RootElement
                .GetProperty("error")
                .GetProperty("message")
                .GetString();

            return message switch
            {
                "EMAIL_NOT_FOUND" => "No account found with this email address.",
                "INVALID_PASSWORD" => "Incorrect password.",
                "INVALID_EMAIL" => "The email address is not valid.",
                "USER_DISABLED" => "This account has been disabled.",
                "TOO_MANY_ATTEMPTS_TRY_LATER" => "Too many failed attempts. Please try again later.",
                _ => message
            };
        }
        catch
        {
            return null;
        }
    }
}

internal sealed class FirebaseIdentityResponse
{
    [JsonPropertyName("idToken")]
    public string IdToken { get; init; } = string.Empty;

    [JsonPropertyName("localId")]
    public string LocalId { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;
}
