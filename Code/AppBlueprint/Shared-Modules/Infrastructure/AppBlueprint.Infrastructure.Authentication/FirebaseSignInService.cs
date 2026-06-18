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
                string errorMessage = "Invalid email or password.";

                if (TryParseFirebaseError(responseBody, out FirebaseErrorDetails errorDetails))
                {
                    _logger.LogWarning(
                        "Firebase sign-in failed with status: {StatusCode}. Error: {Error}. ErrorDescription: {ErrorDescription}",
                        response.StatusCode,
                        errorDetails.Error,
                        errorDetails.ErrorDescription);

                    errorMessage = errorDetails.UserMessage ?? errorMessage;
                }
                else
                {
                    _logger.LogWarning(
                        "Firebase sign-in failed with status: {StatusCode}. Unable to parse error response body.",
                        response.StatusCode);
                }

                return new FirebaseSignInResult { IsSuccess = false, Error = errorMessage };
            }

            var body = JsonSerializer.Deserialize<FirebaseIdentityResponse>(responseBody, JsonOptions);

            if (body is null || string.IsNullOrEmpty(body.IdToken))
            {
                _logger.LogError("Firebase returned unexpected response from authentication service");
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
            _logger.LogError(ex, "Network error during Firebase sign-in");
            return new FirebaseSignInResult { IsSuccess = false, Error = "A network error occurred. Please check your connection." };
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Firebase sign-in request timed out");
            return new FirebaseSignInResult { IsSuccess = false, Error = "The request timed out. Please try again." };
        }
    }

    private static bool TryParseFirebaseError(string json, out FirebaseErrorDetails errorDetails)
    {
        errorDetails = new FirebaseErrorDetails(null, null, null);

        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

#pragma warning disable CA1031 // Generic catch for error parsing - use default message if JSON parsing fails
        try
        {
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("error", out JsonElement errorElement))
            {
                return false;
            }

            string? error = null;
            string? errorDescription = null;

            if (errorElement.ValueKind == JsonValueKind.Object)
            {
                error = errorElement.TryGetProperty("message", out JsonElement messageElement) && messageElement.ValueKind == JsonValueKind.String
                    ? messageElement.GetString()
                    : null;

                if (errorElement.TryGetProperty("errors", out JsonElement errorsElement) && errorsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement item in errorsElement.EnumerateArray())
                    {
                        if (item.ValueKind != JsonValueKind.Object)
                        {
                            continue;
                        }

                        if (item.TryGetProperty("reason", out JsonElement reasonElement) && reasonElement.ValueKind == JsonValueKind.String)
                        {
                            errorDescription = reasonElement.GetString();
                            break;
                        }
                    }
                }
            }
            else if (errorElement.ValueKind == JsonValueKind.String)
            {
                error = errorElement.GetString();
            }

            if (string.IsNullOrEmpty(error) && string.IsNullOrEmpty(errorDescription))
            {
                return false;
            }

            errorDetails = new FirebaseErrorDetails(error, errorDescription, MapFirebaseErrorMessage(error));
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
#pragma warning restore CA1031
    }

    private static string? MapFirebaseErrorMessage(string? error)
    {
        return error switch
        {
            "EMAIL_NOT_FOUND" => "No account found with this email address.",
            "INVALID_PASSWORD" => "Incorrect password.",
            "INVALID_EMAIL" => "The email address is not valid.",
            "USER_DISABLED" => "This account has been disabled.",
            "TOO_MANY_ATTEMPTS_TRY_LATER" => "Too many failed attempts. Please try again later.",
            { Length: > 0 } => error,
            _ => null
        };
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

internal sealed record FirebaseErrorDetails(string? Error, string? ErrorDescription, string? UserMessage);
