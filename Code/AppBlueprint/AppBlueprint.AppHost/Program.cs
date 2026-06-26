// ReSharper disable LocalizableElement
using System.Net;
using System.Net.Sockets;

const string DatabaseConnectionStringKey = "DATABASE_CONNECTIONSTRING";
const string LogtoEndpointKey = "LOGTO_ENDPOINT";
const string LogtoAppIdKey = "LOGTO_APPID";
const string LogtoAppSecretKey = "LOGTO_APPSECRET";
const string LogtoApiResourceKey = "LOGTO_APIRESOURCE";
const string AuthenticationProviderKey = "AUTHENTICATION_PROVIDER";
const string DatabaseContextTypeKey = "DATABASECONTEXT_TYPE";
const string DatabaseContextEnableHybridModeKey = "DATABASECONTEXT_ENABLEHYBRIDMODE";
const string AspNetCoreUrlsKey = "ASPNETCORE_URLS";

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

// --- DATABASE CONFIGURATION ---
// Use Neon.tech/Doppler DATABASE_CONNECTIONSTRING if set; otherwise start local Docker PostgreSQL.
var databaseConnectionString = Environment.GetEnvironmentVariable(DatabaseConnectionStringKey);
IResourceBuilder<PostgresDatabaseResource>? localDb = null;

if (string.IsNullOrEmpty(databaseConnectionString))
{
    Console.WriteLine("[AppHost] DATABASE_CONNECTIONSTRING not set — starting local Docker PostgreSQL");
    var postgres = builder.AddPostgres("postgres")
        .WithPgAdmin();
    localDb = postgres.AddDatabase("appblueprintdb");
}
else
{
    bool hasPassword = HasPassword(databaseConnectionString);
    Console.WriteLine($"[AppHost] DATABASE_CONNECTIONSTRING loaded - Contains Password: {hasPassword}");
    if (!hasPassword)
        Console.WriteLine("[AppHost] WARNING: Database connection string is missing password! Check Doppler secrets.");
}

// Read Logto configuration from environment variables (Doppler)
string? logtoEndpoint = Environment.GetEnvironmentVariable(LogtoEndpointKey);
string? logtoAppId = Environment.GetEnvironmentVariable(LogtoAppIdKey);
string? logtoAppSecret = Environment.GetEnvironmentVariable(LogtoAppSecretKey);
string? logtoApiResource = Environment.GetEnvironmentVariable(LogtoApiResourceKey);
string? authenticationProvider = Environment.GetEnvironmentVariable(AuthenticationProviderKey);
string? databaseContextType = Environment.GetEnvironmentVariable(DatabaseContextTypeKey);
string? databaseContextEnableHybridMode = Environment.GetEnvironmentVariable(DatabaseContextEnableHybridModeKey) ?? "true";

builder.AddProject<Projects.AppBlueprint_AppGateway>("appgw")
    .WithHttpEndpoint(port: 9000, name: "gateway", isProxied: false)
    .WithEnvironment(AspNetCoreUrlsKey, "http://0.0.0.0:9000");

var apiService = builder.AddProject<Projects.AppBlueprint_ApiService>("apiservice")
    .WithHttpEndpoint(port: 9100, name: "api", isProxied: false)
    .WithEnvironment(AspNetCoreUrlsKey, "http://0.0.0.0:9100")
    .WithEnvironment("SWAGGER_PATH", "/swagger");

if (localDb is not null)
    apiService = apiService.WithReference(localDb).WaitFor(localDb);
else
    apiService = apiService.WithEnvironment(DatabaseConnectionStringKey, databaseConnectionString!);

if (!string.IsNullOrWhiteSpace(logtoEndpoint))
    apiService = apiService.WithEnvironment(LogtoEndpointKey, logtoEndpoint);
if (!string.IsNullOrWhiteSpace(logtoAppId))
    apiService = apiService.WithEnvironment(LogtoAppIdKey, logtoAppId);
if (!string.IsNullOrWhiteSpace(logtoAppSecret))
    apiService = apiService.WithEnvironment(LogtoAppSecretKey, logtoAppSecret);
if (!string.IsNullOrWhiteSpace(logtoApiResource))
    apiService = apiService.WithEnvironment(LogtoApiResourceKey, logtoApiResource);
if (!string.IsNullOrWhiteSpace(authenticationProvider))
    apiService = apiService.WithEnvironment(AuthenticationProviderKey, authenticationProvider);
if (!string.IsNullOrWhiteSpace(databaseContextType))
    apiService = apiService.WithEnvironment(DatabaseContextTypeKey, databaseContextType);
if (!string.IsNullOrWhiteSpace(databaseContextEnableHybridMode))
    apiService = apiService.WithEnvironment(DatabaseContextEnableHybridModeKey, databaseContextEnableHybridMode);

var webFrontend = builder.AddProject<Projects.AppBlueprint_Web>("webfrontend")
    .WithHttpEndpoint(port: 9200, name: "web-http", isProxied: false)
    .WithReference(apiService)
    .WithEnvironment(AspNetCoreUrlsKey, "http://0.0.0.0:9200")
    // Using dynamic IP so mobile devices can reach the API on this host machine
    .WithEnvironment("API_BASE_URL", $"http://{localIp}:9100");

if (localDb is not null)
    webFrontend = webFrontend.WithReference(localDb).WaitFor(localDb);
else
    webFrontend = webFrontend.WithEnvironment(DatabaseConnectionStringKey, databaseConnectionString!);

if (!string.IsNullOrWhiteSpace(logtoEndpoint))
    webFrontend = webFrontend.WithEnvironment(LogtoEndpointKey, logtoEndpoint);
if (!string.IsNullOrWhiteSpace(logtoAppId))
    webFrontend = webFrontend.WithEnvironment(LogtoAppIdKey, logtoAppId);
if (!string.IsNullOrWhiteSpace(logtoAppSecret))
    webFrontend = webFrontend.WithEnvironment(LogtoAppSecretKey, logtoAppSecret);
if (!string.IsNullOrWhiteSpace(authenticationProvider))
    webFrontend = webFrontend.WithEnvironment(AuthenticationProviderKey, authenticationProvider);
if (!string.IsNullOrWhiteSpace(databaseContextType))
    webFrontend = webFrontend.WithEnvironment(DatabaseContextTypeKey, databaseContextType);
if (!string.IsNullOrWhiteSpace(databaseContextEnableHybridMode))
    webFrontend = webFrontend.WithEnvironment(DatabaseContextEnableHybridModeKey, databaseContextEnableHybridMode);
if (!string.IsNullOrWhiteSpace(logtoApiResource))
    webFrontend = webFrontend.WithEnvironment(LogtoApiResourceKey, logtoApiResource);

await builder.Build().RunAsync();
