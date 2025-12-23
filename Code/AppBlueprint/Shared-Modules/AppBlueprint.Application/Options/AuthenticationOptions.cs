namespace AppBlueprint.Application.Options;

/// <summary>
/// Configuration options for authentication providers
/// </summary>
public sealed class AuthenticationOptions
{
    /// <summary>
    /// The section name in appsettings.json
    /// </summary>
    public const string SectionName = "Authentication";

    /// <summary>
    /// The authentication provider to use (Logto, Auth0, AzureAD, Cognito, Firebase, JWT, Mock)
    /// </summary>
    public string Provider { get; set; } = "Mock";

    /// <summary>
    /// Logto authentication configuration
    /// </summary>
    public LogtoOptions? Logto { get; set; }

    /// <summary>
    /// Auth0 authentication configuration
    /// </summary>
    public Auth0Options? Auth0 { get; set; }

    /// <summary>
    /// Azure AD B2C authentication configuration
    /// </summary>
    public AzureADOptions? AzureAD { get; set; }

    /// <summary>
    /// AWS Cognito authentication configuration
    /// </summary>
    public CognitoOptions? Cognito { get; set; }

    /// <summary>
    /// Firebase authentication configuration
    /// </summary>
    public FirebaseOptions? Firebase { get; set; }

    /// <summary>
    /// Simple JWT authentication configuration (for testing/development)
    /// </summary>
    public JwtOptions? JWT { get; set; }
}

/// <summary>
/// Logto authentication provider configuration
/// </summary>
public sealed class LogtoOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Scope { get; set; } = "openid profile email";
    public string? Resource { get; set; }
}

/// <summary>
/// Auth0 authentication provider configuration
/// </summary>
public sealed class Auth0Options
{
    public string Domain { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Scope { get; set; } = "openid profile email";
}

/// <summary>
/// Azure AD B2C authentication provider configuration
/// </summary>
public sealed class AzureADOptions
{
    public string Instance { get; set; } = "https://login.microsoftonline.com/";
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string SignUpSignInPolicyId { get; set; } = "B2C_1_signupsignin1";
    public string Scope { get; set; } = "openid profile email";
}

/// <summary>
/// AWS Cognito authentication provider configuration
/// </summary>
public sealed class CognitoOptions
{
    public string UserPoolId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public string Domain { get; set; } = string.Empty;
}

/// <summary>
/// Firebase authentication provider configuration
/// </summary>
public sealed class FirebaseOptions
{
    public string ProjectId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string AuthDomain { get; set; } = string.Empty;
    public string DatabaseUrl { get; set; } = string.Empty;
    public string StorageBucket { get; set; } = string.Empty;
    public string MessagingSenderId { get; set; } = string.Empty;
    public string AppId { get; set; } = string.Empty;
    public string? MeasurementId { get; set; }
}

/// <summary>
/// Simple JWT authentication configuration (for testing/development)
/// </summary>
public sealed class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "AppBlueprint";
    public string Audience { get; set; } = "AppBlueprintClient";
    public int ExpirationMinutes { get; set; } = 60;
}
