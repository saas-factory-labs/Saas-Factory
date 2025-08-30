using System.Text.Json;
using Spectre.Console;

namespace AppBlueprint.DeveloperCli.Utilities;

internal static class RouteScanner
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string DefaultBaseUrl = "https://localhost:7001";
    private const string TodoControllerName = "TodoController";

    public static async Task<List<RouteInfo>> GetAllRoutesAsync(string? baseUrl = null)
    {
        using var httpClient = new HttpClient();

        try
        {
            // Default to localhost development URL if not provided
            baseUrl ??= DefaultBaseUrl;

            var uri = new Uri($"{baseUrl}/dev/routes");
            var response = await httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var routes = JsonSerializer.Deserialize<List<RouteDebugResponse>>(jsonContent, _jsonOptions);

                return routes?.Select(r => new RouteInfo
                {
                    Method = r.Method ?? "UNKNOWN",
                    Path = r.Route ?? "UNKNOWN",
                    Controller = r.Handler
                }).ToList() ?? new List<RouteInfo>();
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Failed to fetch routes: {response.StatusCode}[/]");
                return GetFallbackRoutes();
            }
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]HTTP error fetching routes: {ex.Message}[/]");
            AnsiConsole.MarkupLine("[yellow]Make sure the API is running and accessible.[/]");
            return GetFallbackRoutes();
        }
        catch (JsonException ex)
        {
            AnsiConsole.MarkupLine($"[red]JSON parsing error: {ex.Message}[/]");
            return GetFallbackRoutes();
        }
        catch (UriFormatException ex)
        {
            AnsiConsole.MarkupLine($"[red]Invalid URL format: {ex.Message}[/]");
            return GetFallbackRoutes();
        }
    }

    public static List<RouteInfo> GetAllRoutes(string? baseUrl = null)
    {
        return Task.Run(async () => await GetAllRoutesAsync(baseUrl)).GetAwaiter().GetResult();
    }

    private static List<RouteInfo> GetFallbackRoutes()
    {
        return new List<RouteInfo>
        {
            new RouteInfo { Method = "GET", Path = "/api/todo", Controller = TodoControllerName },
            new RouteInfo { Method = "POST", Path = "/api/todo", Controller = TodoControllerName },
            new RouteInfo { Method = "PUT", Path = "/api/todo/{id}", Controller = TodoControllerName },
            new RouteInfo { Method = "DELETE", Path = "/api/todo/{id}", Controller = TodoControllerName }
        };
    }
}

internal sealed class RouteDebugResponse
{
    public string? Method { get; set; }
    public string? Route { get; set; }
    public string? Handler { get; set; }
}

internal sealed class RouteInfo
{
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
}
