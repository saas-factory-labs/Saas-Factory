using System.Net;
using System.Net.Sockets;

const string LogtoApiResourceKey = "LOGTO_APIRESOURCE";
const string DatabaseContextTypeKey = "DATABASECONTEXT_TYPE";
const string DatabaseContextEnableHybridModeKey = "DATABASECONTEXT_ENABLEHYBRIDMODE";

var builder = DistributedApplication.CreateBuilder(args);

static string GetLocalIpAddress()
{
    using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
    // No actual connection is made; this identifies the primary network interface
    socket.Connect("8.8.8.8", 65530);
    if (socket.LocalEndPoint is IPEndPoint endpoint)
    {
        return endpoint.Address.ToString();
    }

    return "localhost";
}

var localIp = GetLocalIpAddress();

static bool HasPassword(string connectionString)
{
    // Check key-value format: Password=...
    if (connectionString.Contains("Password=", StringComparison.OrdinalIgnoreCase))
        return true;
    
    // Check PostgreSQL URI format: postgresql://username:password@host:port/database
    if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
        connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
    {
        int schemeEnd = connectionString.IndexOf("://", StringComparison.Ordinal);
        int atIndex = connectionString.IndexOf('@', schemeEnd + 3);
        int colonIndex = connectionString.IndexOf(':', schemeEnd + 3);
        
        // Password exists if there's a colon between :// and @
        return colonIndex > schemeEnd && colonIndex < atIndex && atIndex > 0;
    }
    
    return false;
}

// --- DATABASE CONFIGURATION (Doppler & Railway) ---
// Database connection string must be set as DATABASE_CONNECTIONSTRING environment variable
var databaseConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING")
    ?? throw new InvalidOperationException(
        "Database connection string not found. Set DATABASE_CONNECTIONSTRING environment variable. Ensure Doppler is running (doppler run).");

// Diagnostic: Verify password is present (security: don't log actual value)
bool hasPassword = HasPassword(databaseConnectionString);
Console.WriteLine($"[AppHost] DATABASE_CONNECTIONSTRING loaded - Contains Password: {hasPassword}");
if (!hasPassword)
{
    Console.WriteLine("[AppHost] WARNING: Database connection string is missing password! Check Doppler secrets.");
}

// Read Logto configuration from environment variables (Doppler)
string? logtoEndpoint = Environment.GetEnvironmentVariable("LOGTO_ENDPOINT");
string? logtoAppId = Environment.GetEnvironmentVariable("LOGTO_APPID");
string? logtoAppSecret = Environment.GetEnvironmentVariable("LOGTO_APPSECRET");
string? logtoApiResource = Environment.GetEnvironmentVariable(LogtoApiResourceKey);
string? authenticationProvider = Environment.GetEnvironmentVariable("AUTHENTICATION_PROVIDER");
string? databaseContextType = Environment.GetEnvironmentVariable(DatabaseContextTypeKey);
string? databaseContextEnableHybridMode = Environment.GetEnvironmentVariable(DatabaseContextEnableHybridModeKey) ?? "true";

builder.AddProject<Projects.AppBlueprint_AppGateway>("appgw")
    .WithHttpEndpoint(port: 9000, name: "gateway", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://0.0.0.0:9000");

var apiService = builder.AddProject<Projects.AppBlueprint_ApiService>("apiservice")
    .WithHttpEndpoint(port: 9100, name: "api", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://0.0.0.0:9100")
    .WithEnvironment("SWAGGER_PATH", "/swagger")
    .WithEnvironment("DATABASE_CONNECTIONSTRING", databaseConnectionString);

// Add Logto configuration if provided via Doppler
if (!string.IsNullOrWhiteSpace(logtoEndpoint))
    apiService = apiService.WithEnvironment("LOGTO_ENDPOINT", logtoEndpoint);
if (!string.IsNullOrWhiteSpace(logtoAppId))
    apiService = apiService.WithEnvironment("LOGTO_APPID", logtoAppId);
if (!string.IsNullOrWhiteSpace(logtoApiResource))
    apiService = apiService.WithEnvironment(LogtoApiResourceKey, logtoApiResource);
if (!string.IsNullOrWhiteSpace(authenticationProvider))
    apiService = apiService.WithEnvironment("AUTHENTICATION_PROVIDER", authenticationProvider);
if (!string.IsNullOrWhiteSpace(databaseContextType))
    apiService = apiService.WithEnvironment(DatabaseContextTypeKey, databaseContextType);
if (!string.IsNullOrWhiteSpace(databaseContextEnableHybridMode))
    apiService = apiService.WithEnvironment(DatabaseContextEnableHybridModeKey, databaseContextEnableHybridMode);

var webFrontend = builder.AddProject<Projects.AppBlueprint_Web>("webfrontend")
    .WithHttpEndpoint(port: 9200, name: "web-http", isProxied: false)
    .WithReference(apiService)
    .WithEnvironment("ASPNETCORE_URLS", "http://0.0.0.0:9200")
    // Using dynamic IP so mobile devices can reach the API on this host machine
    .WithEnvironment("API_BASE_URL", $"http://{localIp}:9100")
    .WithEnvironment("DATABASE_CONNECTIONSTRING", databaseConnectionString);

// Add Logto configuration if provided via Doppler
if (!string.IsNullOrWhiteSpace(logtoEndpoint))
    webFrontend = webFrontend.WithEnvironment("LOGTO_ENDPOINT", logtoEndpoint);
if (!string.IsNullOrWhiteSpace(logtoAppId))
    webFrontend = webFrontend.WithEnvironment("LOGTO_APPID", logtoAppId);
if (!string.IsNullOrWhiteSpace(logtoAppSecret))
    webFrontend = webFrontend.WithEnvironment(LogtoApiResourceKey, logtoApiResource);
if (!string.IsNullOrWhiteSpace(authenticationProvider))
    webFrontend = webFrontend.WithEnvironment("AUTHENTICATION_PROVIDER", authenticationProvider);
if (!string.IsNullOrWhiteSpace(databaseContextType))
    webFrontend = webFrontend.WithEnvironment(DatabaseContextTypeKey, databaseContextType);
if (!string.IsNullOrWhiteSpace(databaseContextEnableHybridMode))
    webFrontend = webFrontend.WithEnvironment(DatabaseContextEnableHybridModeKey, databaseContextEnableHybridMode);
webFrontend = webFrontend.WithEnvironment("LOGTO_APPSECRET", logtoAppSecret);
if (!string.IsNullOrWhiteSpace(logtoApiResource))
    webFrontend = webFrontend.WithEnvironment(LogtoApiResourceKey, logtoApiResource);
if (!string.IsNullOrWhiteSpace(databaseContextType))
    _ = webFrontend.WithEnvironment(DatabaseContextTypeKey, databaseContextType);

await builder.Build().RunAsync();
